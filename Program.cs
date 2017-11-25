using System;
using log4net;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NDesk.Options;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.Globalization;
namespace WeeklyReports
{
    class Program
    {
        public static string connectionString;
        public static readonly ILog LOG = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {

            var p = new OptionSet
            {
                { "c|HostName=", "[OPTIONAL] Company from which to generate report. example: \"Disney\"", v => Settings.CompanyName = v },
                 { "p|Portal=", "[OPTIONAL] Portal from which to generate report. example: \"Google\"", v => Settings.Portal = v },
                 { "r|Region=", "[OPTIONAL] Region from which to generate report. example: \"US\"", v => Settings.Region = v },
                  { "db|DBName=", "[OPTIONAL] DBName from which to generate report. example: \"dash_paramount_production\"", v => Settings.DbName = v },
                  { "t|Template_File=", "[OPTIONAL] Template from which to generate report. example: \"ScrubM_Bing_US_Disney_ExecutiveSummary\"", v => Settings.FileTemplate = v },
            };

            List<string> extra = p.Parse(args);
            if (extra.Count > 0)
            {
                throw new ArgumentException("Unrecognized command line arguments - " + string.Join(" ", extra.ToArray()));
            }
            Settings.DbName = Settings.DbName.ToLower();
            connectionString = string.Format("server={3};port=3306;database={2};user={0};password={1};default command timeout=0;", Settings.DbUser, Settings.DbPassword, Settings.DbName, Settings.DbServer);

            List<ReportData> data = GetData();
            List<ReportData> action = GetActionData();
            List<ReportData> website = GetWebsiteData();

            QueryGeneration q = new QueryGeneration();
            List<ReportData> top25keywords = q.GetTop25Keywords();
            List<ReportData> top25keywordtemplates = q.GetTop25KeywordTemplates();
            List<ReportData> top25website = q.GetTop25Website();
            List<ReportData> top25new_website = q.GetTop25NewWebsite();
            PrepareReport(data, action, website, top25keywords, top25keywordtemplates, top25website,top25new_website);
        }
        public static List<ReportData> GetData()
        {

            MySqlConnection cn = new MySqlConnection(connectionString);

            cn.Open();

            string[] query;
            string Company_Name = Settings.CompanyName.First().ToString().ToUpper() + Settings.CompanyName.Substring(1);
            if (Settings.Region == null)
            {
                string path = Directory.GetCurrentDirectory();
                string path1 = path.TrimEnd("\\bin\\Debug".ToCharArray());
                if (Settings.Portal == null)
                {
                    query = File.ReadAllLines(string.Format(@"{1}\Query\{0}_Data.txt", Company_Name, path1));
                }
                else
                {
                    query = File.ReadAllLines(string.Format(@"{2}\Query\{0}_{1}_Data.txt", Company_Name, Settings.Portal, path1));
                }
            }
            else
            {
                string path = Directory.GetCurrentDirectory();
                string path1 = path.TrimEnd("\\bin\\Debug".ToCharArray());
                query = File.ReadAllLines(string.Format(@"{3}\Query\{0}_{1}_{2}_Data.txt", Company_Name, Settings.Portal, Settings.Region, path1));
            }
            string query_ = string.Join(" ", query);
            MySqlCommand cmd = new MySqlCommand(query_, cn);

            List<ReportData> reportDataList = new List<ReportData>();
            LOG.InfoFormat(string.Format("Getting {0} {1} Data", Settings.CompanyName, Settings.Region));
            MySqlDataReader myReader = cmd.ExecuteReader();

            while (myReader.Read())
            {
                ReportData reportData = new ReportData();

                reportData.portal = Convert.ToString(myReader[0]);
                reportData.user_region = Convert.ToString(myReader[1]);
                reportData.search_date = (Convert.ToDateTime(myReader[2])).ToString("MM/dd/yyy HH:mm");
                reportData.year = Convert.ToString(myReader[3]);
                reportData.week_of_year = Convert.ToString(myReader[4]);
                reportData.keyword = Convert.ToString(myReader[5]);
                reportData.ip_owner = Convert.ToString(myReader[6]);
                reportData.product = Convert.ToString(myReader[7]);
                reportData.page_number = Convert.ToString(myReader[8]);
                reportData.status = Convert.ToString(myReader[9]);
                reportData.min_position_authorized = Convert.ToString(myReader[10]);
                reportData.min_position_unauthorized = Convert.ToString(myReader[11]);
                reportData.num_authorized = Convert.ToString(myReader[12]);
                reportData.num_unauthorized = Convert.ToString(myReader[13]);
                reportData.num_results = Convert.ToString(myReader[14]);
                reportData.perc_unauthorized = Convert.ToString(myReader[15]);
                reportData.avg_age_unauthorized_days = Convert.ToString(myReader[16]);
                reportData.keyword_template = Convert.ToString(myReader[17]);
                reportData.position_gap = Convert.ToString(myReader[18]);
                reportData.traffic_share = Convert.ToString(myReader[19]);
                reportData.unauthorized_traffic_share = Convert.ToString(myReader[20]);
                reportData.authorized_traffic_share = Convert.ToString(myReader[21]);
                reportData.used_historical_traffic_data = Convert.ToString(myReader[22]) == "False" ? "0" : "1";
                reportDataList.Add(reportData);
            }
            myReader.Close();
            cn.Close();


            return reportDataList;
        }
        public static List<ReportData> GetActionData()
        {
            MySqlConnection cn = new MySqlConnection(connectionString);

            LOG.InfoFormat(string.Format("Getting {0} {1} Actions", Settings.CompanyName, Settings.Region));
            string Company_Name = Settings.CompanyName.First().ToString().ToUpper() + Settings.CompanyName.Substring(1);
            cn.Open();

            string[] query;
            if (Settings.Region == null && Settings.Portal != null)
            {
                string path = Directory.GetCurrentDirectory();
                string path1 = path.TrimEnd("\\bin\\Debug".ToCharArray());
                query = File.ReadAllLines(string.Format(@"{2}\Query\{0}_{1}_Actions.txt", Company_Name, Settings.Portal, path1));
            }
            else if (Settings.Portal == null)
            {
                string path = Directory.GetCurrentDirectory();
                string path1 = path.TrimEnd("\\bin\\Debug".ToCharArray());
                query = File.ReadAllLines(string.Format(@"{1}\Query\{0}_Actions.txt", Company_Name, path1));

            }
            else
            {
                string path = Directory.GetCurrentDirectory();
                string path1 = path.TrimEnd("\\bin\\Debug".ToCharArray());
                query = File.ReadAllLines(string.Format(@"{3}\Query\{0}_{1}_{2}_Actions.txt", Company_Name, Settings.Portal, Settings.Region, path1));
            }
            string query_ = string.Join(" ", query);
            MySqlCommand cmd = new MySqlCommand(query_, cn);

            List<ReportData> reportDataList = new List<ReportData>();

            MySqlDataReader myReader = cmd.ExecuteReader();

            while (myReader.Read())
            {
                ReportData reportData = new ReportData();

                reportData.portal = Convert.ToString(myReader[0]);
                reportData.user_region = Convert.ToString(myReader[1]);
                reportData.sent_at = (Convert.ToDateTime(myReader[2])).ToString("MM/dd/yyy HH:mm");
                reportData.year = Convert.ToString(myReader[3]);
                reportData.week_of_year = Convert.ToString(myReader[4]);
                reportData.number_reported = Convert.ToString(myReader[5]);
                reportData.number_removed = Convert.ToString(myReader[6]);
                reportData.average_time_to_action_minutes = Convert.ToString(myReader[7]);
                reportData.average_time_to_remove_minutes = Convert.ToString(myReader[8]);
                reportData.sum_time_to_action_Minutes = Convert.ToString(myReader[9]);
                reportData.sum_time_to_remove_minutes = Convert.ToString(myReader[10]);

                reportDataList.Add(reportData);
            }
            myReader.Close();
            cn.Close();

            return reportDataList;
        }
        public static List<ReportData> GetWebsiteData()
        {
            MySqlConnection cn = new MySqlConnection(connectionString);

            LOG.InfoFormat(string.Format("Getting {0} {1} Websites", Settings.CompanyName, Settings.Region));

            cn.Open();

            string[] query;
            string Company_Name = Settings.CompanyName.First().ToString().ToUpper() + Settings.CompanyName.Substring(1);
            if (Settings.Region == null && Settings.Portal != null)
            {
                string path = Directory.GetCurrentDirectory();
                string path1 = path.TrimEnd("\\bin\\Debug".ToCharArray());
                query = File.ReadAllLines(string.Format(@"{2}\Query\{0}_{1}_Websites.txt", Company_Name, Settings.Portal, path1));
            }
            else if (Settings.Portal == null)
            {
                string path = Directory.GetCurrentDirectory();
                string path1 = path.TrimEnd("\\bin\\Debug".ToCharArray());
                query = File.ReadAllLines(string.Format(@"{1}\Query\{0}_Websites.txt", Company_Name, path1));
            }
            else
            {
                string path = Directory.GetCurrentDirectory();
                string path1 = path.TrimEnd("\\bin\\Debug".ToCharArray());
                query = File.ReadAllLines(string.Format(@"{3}\Query\{0}_{1}_{2}_Websites.txt", Company_Name, Settings.Portal, Settings.Region, path1));
            }
            string query_ = string.Join(" ", query);
            MySqlCommand cmd = new MySqlCommand(query_, cn);

            List<ReportData> reportDataList = new List<ReportData>();

            MySqlDataReader myReader = cmd.ExecuteReader();

            while (myReader.Read())
            {
                ReportData reportData = new ReportData();

                reportData.host_name = Convert.ToString(myReader[0]);
                reportData.num_unauthorized = Convert.ToString(myReader[1]);
                reportData.first_unauthorized_search_date = (Convert.ToDateTime(myReader[2])).ToString("MM/dd/yyy HH:mm");
                reportData.first_unauthorized_week_of_year = Convert.ToString(myReader[3]);
                reportData.search_date = (Convert.ToDateTime(myReader[4])).ToString("MM/dd/yyy HH:mm");
                reportData.year = Convert.ToString(myReader[5]);
                reportData.week_of_year = Convert.ToString(myReader[6]);
                reportData.first_unauthorized_year = Convert.ToString(myReader[7]);

                reportDataList.Add(reportData);
            }
            myReader.Close();
            cn.Close();

            return reportDataList;
        }
        private static void PrepareReport(List<ReportData> dataList, List<ReportData> actionList, List<ReportData> websiteList, List<ReportData> top25keywordslist, List<ReportData> top25keywordtemplateslist, List<ReportData> top25websitelist, List<ReportData>  top25new_websiteList)
        {
            LOG.Info(string.Format("ReportGenerator::PrepareReport started to Generate {0} {1} Report", Settings.CompanyName, Settings.Region));
            System.IO.Directory.CreateDirectory(@"C:\Marketly\Weekly Reports");
            string strFilename = Settings.FileTemplate.Substring(0, Settings.FileTemplate.LastIndexOf('.'));
            string fname = (string.Format("{0}_{1}.xlsx", strFilename, Settings.ReportDate.ToString("yyyyMMdd")));

            string path = Directory.GetCurrentDirectory();
            string path1 = path.TrimEnd("\\bin\\Debug".ToCharArray());
            FileInfo templateFile = new FileInfo(string.Format(@"{1}/Template/{0}", Settings.FileTemplate, path1));

            string Company_Name = Settings.CompanyName.First().ToString().ToUpper() + Settings.CompanyName.Substring(1);
            //string fname = string.Format("ScrubM_{0}_{1}_{2}_ExecutiveSummary{3}.xlsx", Settings.Portal, Settings.Region, Company_Name, Settings.ReportDate.ToString("yyyyMMdd"));

            FileInfo outputFile = new FileInfo(Settings.OutputDir + "\\" + fname);

            ExcelPackage xlsxtemplate = new ExcelPackage(outputFile, templateFile);

            ExcelWorksheet data = xlsxtemplate.Workbook.Worksheets["Data"];
            ExcelWorksheet action_data = xlsxtemplate.Workbook.Worksheets["Action Data"];
            ExcelWorksheet website_data = xlsxtemplate.Workbook.Worksheets["Website Data"];
            ExcelWorksheet top25keywordtemplates = xlsxtemplate.Workbook.Worksheets["Top25KeywordTemplates"];
            ExcelWorksheet top25keywords = xlsxtemplate.Workbook.Worksheets["Top25Keywords"];
            ExcelWorksheet top25website = xlsxtemplate.Workbook.Worksheets["Top25Website"];
            ExcelWorksheet top25new_website = xlsxtemplate.Workbook.Worksheets["Top25NewWebsite"];

            WriteDataSheet(data, dataList);
            WriteDataSheet(action_data, actionList);
            WriteDataSheet(website_data, websiteList);

            WriteTop25Keywords(top25keywords, top25keywordslist);
            WriteTop25KeywordTemplates(top25keywordtemplates, top25keywordtemplateslist);
            WriteTop25Website(top25website, top25websitelist);
            WriteTop25Website(top25new_website, top25new_websiteList);
            ExcelWorksheet executive_summary = xlsxtemplate.Workbook.Worksheets["Executive Summary"];

            //executive_summary.Cells["B1:O8"].Merge = true;
            //executive_summary.Cells["B1:O8"].Value= string.Format("ScrubM Weekly Report for {0} {1}\n{2} Executive Summary\nMARKETLY", Company_Name, Settings.Region, Settings.Portal);
            CultureInfo cul = CultureInfo.CurrentCulture;
            executive_summary.Cells[9, 3].Value = cul.Calendar.GetWeekOfYear(System.DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - 1;
            //executive_summary.Cells[239, 13].Value = Settings.Portal;
            //executive_summary.Cells[240, 13].Value = Settings.Region;

            top25keywordtemplates.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;
            top25keywords.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;
            top25website.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;
            top25new_website.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;

            xlsxtemplate.Save();

            LOG.Info(string.Format("ReportGenerator::{0} {1} Report Generate Successfully!!!", Company_Name, Settings.Region));


            // return outputFile.FullName;
        }

        private static void WriteDataSheet(ExcelWorksheet ws, List<ReportData> dataList)
        {
            int rowInit = 2;
            if (ws.ToString() == "Data")
            {
                if (dataList.Count != 0)
                {
                    ws.InsertRow(2, dataList.Count - 1);

                    for (int rowCount = rowInit; rowCount < dataList.Count + rowInit; rowCount++)
                    {
                        int columnCount = 1;
                        try
                        {
                            ReportData currentReportData = dataList[rowCount - rowInit];

                            ws.Cells[rowCount, columnCount].Value = currentReportData.portal;
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = (currentReportData.user_region);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToDateTime(currentReportData.search_date);
                            ws.Cells[rowCount, columnCount].Style.Numberformat.Format = "MM/dd/yyy HH:mm";
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.year);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.week_of_year);
                            columnCount++;

                            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                            encoding.GetBytes(currentReportData.keyword);
                            ws.Cells[rowCount, columnCount].Value = (currentReportData.keyword);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = (currentReportData.ip_owner);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = (currentReportData.product);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.page_number);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = (currentReportData.status);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.min_position_authorized);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.min_position_unauthorized);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.num_authorized);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.num_unauthorized);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.num_results);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = (currentReportData.perc_unauthorized);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = (currentReportData.avg_age_unauthorized_days);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = currentReportData.keyword_template;
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.position_gap);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.traffic_share);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.unauthorized_traffic_share);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.authorized_traffic_share);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.used_historical_traffic_data);
                            columnCount++;

                        }
                        catch (Exception ex)
                        {
                            LOG.ErrorFormat("PrepareReport failed to process due to {0}", ex.Message, ex);
                        }
                    }


                }
            }
            else if (ws.ToString() == "Action Data")
            {


                if (dataList.Count != 0)
                {
                    ws.InsertRow(2, dataList.Count - 1);

                    for (int rowCount = rowInit; rowCount < dataList.Count + rowInit; rowCount++)
                    {
                        int columnCount = 1;
                        try
                        {
                            ReportData currentReportData = dataList[rowCount - rowInit];

                            ws.Cells[rowCount, columnCount].Value = currentReportData.portal;
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = (currentReportData.user_region);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToDateTime(currentReportData.sent_at);
                            ws.Cells[rowCount, columnCount].Style.Numberformat.Format = "MM/dd/yyy HH:mm";
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.year);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.week_of_year);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.number_reported);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.number_removed);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToDouble(currentReportData.average_time_to_action_minutes);
                            columnCount++;
                            if (currentReportData.average_time_to_remove_minutes == null | currentReportData.average_time_to_remove_minutes == "")
                            {
                                ws.Cells[rowCount, columnCount].Value = currentReportData.average_time_to_remove_minutes;
                                columnCount++;
                            }
                            else
                            {
                                ws.Cells[rowCount, columnCount].Value = Convert.ToDouble(currentReportData.average_time_to_remove_minutes);
                                columnCount++;
                            }

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt64(currentReportData.sum_time_to_action_Minutes);
                            columnCount++;

                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt64(currentReportData.sum_time_to_remove_minutes);
                            columnCount++;


                        }
                        catch (Exception ex)
                        {
                            LOG.ErrorFormat("PrepareReport failed to process due to {0}", ex.Message, ex);
                        }
                    }


                }
            }

            else if (ws.ToString() == "Website Data")
            {
                if (dataList.Count != 0)
                {
                    ws.InsertRow(2, dataList.Count - 1);
                    for (int rowCount = rowInit; rowCount < dataList.Count + rowInit; rowCount++)
                    {
                        int columnCount = 1;
                        try
                        {
                            ReportData currentReportData = dataList[rowCount - rowInit];

                            ws.Cells[rowCount, columnCount].Value = currentReportData.host_name;
                            columnCount++;
                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.num_unauthorized);
                            columnCount++;
                            ws.Cells[rowCount, columnCount].Value = Convert.ToDateTime(currentReportData.first_unauthorized_search_date);
                            ws.Cells[rowCount, columnCount].Style.Numberformat.Format = "MM/dd/yyy HH:mm";
                            columnCount++;
                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.first_unauthorized_week_of_year);
                            columnCount++;
                            ws.Cells[rowCount, columnCount].Value = Convert.ToDateTime(currentReportData.search_date);
                            ws.Cells[rowCount, columnCount].Style.Numberformat.Format = "MM/dd/yyy HH:mm";
                            columnCount++;
                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.year);
                            columnCount++;
                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.week_of_year);
                            columnCount++;
                            ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.first_unauthorized_year);
                            columnCount++;
                        }

                        catch (Exception ex)
                        {
                            LOG.ErrorFormat("PrepareReport failed to process due to {0}", ex.Message, ex);
                        }
                    }

                }
                else
                {
                    LOG.Warn("ReportGenerator::Data not found in : " + ws + " sheet");
                }
            }
        }

        private static void WriteTop25Keywords(ExcelWorksheet ws, List<ReportData> dataList)
        {
            if (dataList.Count != 0)
            {
                int rowInit = 2;
                ws.InsertRow(2, dataList.Count);

                for (int rowCount = rowInit; rowCount < dataList.Count + rowInit; rowCount++)
                {
                    int columnCount = 1;

                    try
                    {
                        ReportData currentReportData = dataList[rowCount - rowInit];

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.sum_of_numunauthorized);
                        columnCount++;

                        System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                        encoding.GetBytes(currentReportData.keyword);
                        ws.Cells[rowCount, columnCount].Value = (currentReportData.keyword);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToString(currentReportData.keyword_template);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.week_of_year);
                        columnCount++;


                        ws.Cells[rowCount, columnCount].Value = (currentReportData.portal);
                        columnCount++;


                        ws.Cells[rowCount, columnCount].Value = (currentReportData.user_region);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToDateTime(currentReportData.search_date);
                        ws.Cells[rowCount, columnCount].Style.Numberformat.Format = "MM/dd/yyy HH:mm";
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.year);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = (currentReportData.ip_owner);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = (currentReportData.product);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.page_number);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = (currentReportData.status);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.min_position_authorized);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.min_position_unauthorized);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.num_authorized);
                        columnCount++;


                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.num_results);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = (currentReportData.perc_unauthorized);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = (currentReportData.avg_age_unauthorized_days);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.position_gap);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.traffic_share);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.unauthorized_traffic_share);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.authorized_traffic_share);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.used_historical_traffic_data);
                        columnCount++;

                    }
                    catch (Exception ex)
                    {
                        LOG.ErrorFormat("PrepareReport failed to process due to {0}", ex.Message, ex);
                    }
                }
            }
        }

        private static void WriteTop25KeywordTemplates(ExcelWorksheet ws, List<ReportData> dataList)
        {
            if (dataList.Count != 0)
            {
                int rowInit = 2;
                ws.InsertRow(2, dataList.Count);

                for (int rowCount = rowInit; rowCount < dataList.Count + rowInit; rowCount++)
                {
                    int columnCount = 1;

                    try
                    {
                        ReportData currentReportData = dataList[rowCount - rowInit];

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.sum_of_numunauthorized);
                        columnCount++;

                        System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                        encoding.GetBytes(currentReportData.keyword);
                        ws.Cells[rowCount, columnCount].Value = (currentReportData.keyword);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToString(currentReportData.keyword_template);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.week_of_year);
                        columnCount++;


                        ws.Cells[rowCount, columnCount].Value = (currentReportData.portal);
                        columnCount++;


                        ws.Cells[rowCount, columnCount].Value = (currentReportData.user_region);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToDateTime(currentReportData.search_date);
                        ws.Cells[rowCount, columnCount].Style.Numberformat.Format = "MM/dd/yyy HH:mm";
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.year);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = (currentReportData.ip_owner);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = (currentReportData.product);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.page_number);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = (currentReportData.status);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.min_position_authorized);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.min_position_unauthorized);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.num_authorized);
                        columnCount++;


                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.num_results);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = (currentReportData.perc_unauthorized);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = (currentReportData.avg_age_unauthorized_days);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.position_gap);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.traffic_share);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.unauthorized_traffic_share);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.authorized_traffic_share);
                        columnCount++;

                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.used_historical_traffic_data);
                        columnCount++;

                    }
                    catch (Exception ex)
                    {
                        LOG.ErrorFormat("PrepareReport failed to process due to {0}", ex.Message, ex);
                    }
                }
            }
        }

        private static void WriteTop25Website(ExcelWorksheet ws, List<ReportData> dataList)
        {
            int rowInit = 2;
            if (dataList.Count != 0)
            {
                ws.InsertRow(2, dataList.Count - 1);
                for (int rowCount = rowInit; rowCount < dataList.Count + rowInit; rowCount++)
                {
                    int columnCount = 1;
                    try
                    {
                        ReportData currentReportData = dataList[rowCount - rowInit];

                        ws.Cells[rowCount, columnCount].Value = currentReportData.host_name;
                        columnCount++;
                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.num_unauthorized);
                        columnCount++;
                        ws.Cells[rowCount, columnCount].Value = Convert.ToDateTime(currentReportData.first_unauthorized_search_date);
                        ws.Cells[rowCount, columnCount].Style.Numberformat.Format = "MM/dd/yyy HH:mm";
                        columnCount++;
                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.first_unauthorized_week_of_year);
                        columnCount++;
                        ws.Cells[rowCount, columnCount].Value = Convert.ToDateTime(currentReportData.search_date);
                        ws.Cells[rowCount, columnCount].Style.Numberformat.Format = "MM/dd/yyy HH:mm";
                        columnCount++;
                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.year);
                        columnCount++;
                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.week_of_year);
                        columnCount++;
                        ws.Cells[rowCount, columnCount].Value = Convert.ToInt32(currentReportData.first_unauthorized_year);
                        columnCount++;
                    }

                    catch (Exception ex)
                    {
                        LOG.ErrorFormat("PrepareReport failed to process due to {0}", ex.Message, ex);
                    }
                }

            }
            else
            {
                LOG.Warn("ReportGenerator::Data not found in : " + ws + " sheet");
            }
        }
    }    
}