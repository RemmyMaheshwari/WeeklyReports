using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace WeeklyReports
{
    class QueryGeneration
    {
        public string Top25DataQuery(string filename)
        {
            int current_year = (System.DateTime.Now.Year);
            int week_of_year;

            CultureInfo cul = CultureInfo.CurrentCulture;
            week_of_year = cul.Calendar.GetWeekOfYear(System.DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - 1;
            string path = Directory.GetCurrentDirectory();
            string path1 = path.TrimEnd("\\bin\\Debug".ToCharArray());
            path1 += @"\Top_25_Queries\" + filename;
            string[] query = File.ReadAllLines(path1);
            string query_ = string.Join(" ", query);
            query_ = string.Format(query_, week_of_year.ToString(), (week_of_year - 1).ToString(), (week_of_year - 2).ToString(), (week_of_year - 3).ToString(), (week_of_year - 4).ToString(), (week_of_year - 5).ToString(), (week_of_year - 6).ToString(), (current_year).ToString(), Settings.Portal == null ? "" : "and portal='" + Settings.Portal + "'", Settings.Region == null ? "" : "and region='" + Settings.Region + "'");
            return query_;
        }

        public List<ReportData> GetTop25Keywords()
        {
            string query = Top25DataQuery("top_25_keyword.txt");
            MySqlConnection cn = new MySqlConnection(Program.connectionString);

            cn.Open();

            List<ReportData> data = new List<ReportData>();

            MySqlCommand cmd = new MySqlCommand(query, cn);

            List<ReportData> reportDataList = new List<ReportData>();
            Program.LOG.InfoFormat(string.Format("Getting {0} {1} Top 25 Data", Settings.CompanyName, Settings.Region));
            MySqlDataReader myReader = cmd.ExecuteReader();

            while (myReader.Read())
            {
                ReportData reportData = new ReportData();
                reportData.sum_of_numunauthorized = Convert.ToString(myReader[0]);
                reportData.keyword = Convert.ToString(myReader[1]);
                reportData.keyword_template = Convert.ToString(myReader[2]);
                reportData.week_of_year = Convert.ToString(myReader[3]);
                reportData.portal = Convert.ToString(myReader[4]);
                reportData.user_region = Convert.ToString(myReader[5]);
                reportData.search_date = (Convert.ToDateTime(myReader[6])).ToString("MM/dd/yyy HH:mm");
                reportData.year = Convert.ToString(myReader[7]);
                reportData.ip_owner = Convert.ToString(myReader[8]);
                reportData.product = Convert.ToString(myReader[9]);
                reportData.page_number = Convert.ToString(myReader[10]);
                reportData.status = Convert.ToString(myReader[11]);
                reportData.min_position_authorized = Convert.ToString(myReader[12]);
                reportData.min_position_unauthorized = Convert.ToString(myReader[13]);
                reportData.num_authorized = Convert.ToString(myReader[14]);
                reportData.num_unauthorized = Convert.ToString(myReader[13]);
                reportData.num_results = Convert.ToString(myReader[15]);
                reportData.perc_unauthorized = Convert.ToString(myReader[16]);
                reportData.avg_age_unauthorized_days = Convert.ToString(myReader[17]);

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

        public List<ReportData> GetTop25KeywordTemplates()
        {
            string query = Top25DataQuery("top_25_keyword_template.txt");
            MySqlConnection cn = new MySqlConnection(Program.connectionString);

            cn.Open();

            List<ReportData> data = new List<ReportData>();

            MySqlCommand cmd = new MySqlCommand(query, cn);

            List<ReportData> reportDataList = new List<ReportData>();
            Program.LOG.InfoFormat(string.Format("Getting {0} {1} Top 25 Keyword Templates", Settings.CompanyName, Settings.Region));
            MySqlDataReader myReader = cmd.ExecuteReader();

            while (myReader.Read())
            {
                ReportData reportData = new ReportData();
                reportData.sum_of_numunauthorized = Convert.ToString(myReader[0]);
                reportData.keyword = Convert.ToString(myReader[1]);
                reportData.keyword_template = Convert.ToString(myReader[2]);
                reportData.week_of_year = Convert.ToString(myReader[3]);
                reportData.portal = Convert.ToString(myReader[4]);
                reportData.user_region = Convert.ToString(myReader[5]);
                reportData.search_date = (Convert.ToDateTime(myReader[6])).ToString("MM/dd/yyy HH:mm");
                reportData.year = Convert.ToString(myReader[7]);
                reportData.ip_owner = Convert.ToString(myReader[8]);
                reportData.product = Convert.ToString(myReader[9]);
                reportData.page_number = Convert.ToString(myReader[10]);
                reportData.status = Convert.ToString(myReader[11]);
                reportData.min_position_authorized = Convert.ToString(myReader[12]);
                reportData.min_position_unauthorized = Convert.ToString(myReader[13]);
                reportData.num_authorized = Convert.ToString(myReader[14]);
                reportData.num_unauthorized = Convert.ToString(myReader[13]);
                reportData.num_results = Convert.ToString(myReader[15]);
                reportData.perc_unauthorized = Convert.ToString(myReader[16]);
                reportData.avg_age_unauthorized_days = Convert.ToString(myReader[17]);

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

        public List<ReportData> GetTop25Website()
        {
            string query = Top25DataQuery("top_25_website.txt");
            Program.LOG.InfoFormat(string.Format("Getting {0} {1} Top 25 Websites Data", Settings.CompanyName, Settings.Region));
            MySqlConnection cn = new MySqlConnection(Program.connectionString);
            MySqlCommand cmd = new MySqlCommand(query, cn);

            List<ReportData> reportDataList = new List<ReportData>();

            cn.Open();
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

        public List<ReportData> GetTop25NewWebsite()
        {
            string query = Top25DataQuery("top_25_new_website.txt");
            Program.LOG.InfoFormat(string.Format("Getting {0} {1} Top 25 New Websites Data", Settings.CompanyName, Settings.Region));
            MySqlConnection cn = new MySqlConnection(Program.connectionString);
            MySqlCommand cmd = new MySqlCommand(query, cn);

            List<ReportData> reportDataList = new List<ReportData>();

            cn.Open();
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
    }
}
