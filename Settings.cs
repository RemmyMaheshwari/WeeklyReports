using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeeklyReports
{
    class Settings
    {
        public static string ReportTemplate { get; } = "ScrubM_Bing_US_Disney_ExecutiveSummary.xlsx";

        public static string SubDirectory { get; set; }
        public static string Query { get; set; }
        public static string OutputDir { get; set; }= ConfigurationManager.AppSettings["OutputDir"];
        public static DateTime ReportDate { get; } = System.DateTime.Now;
        public static string FileTemplate { get; set; } 
      //  public static string OutputFileName { get; set; } = string.Format(@"ScrubM_,{0}_{1}_{2}_ExecutiveSummary",);




        //Db authentication
        public static string DbUser { get; } = ConfigurationManager.AppSettings["DbUser"];
        public static string DbPassword { get; } = ConfigurationManager.AppSettings["DbPassword"];
        public static string DbName { get; set; }
        public static string CompanyName { get; set; }
        public static string Portal { get; set; }
        public static string Region { get; set; }
        //public static string DbNameUnapproved { get; } = ConfigurationManager.AppSettings["DbNameUnapproved"];
        public static string DbServer { get; } = ConfigurationManager.AppSettings["DbServer"];

        public static string Environment()
        {
            return "production";
        }

        /// <summary>
        /// Gets SmtpServer name to send the message to
        /// </summary>
        public static string SmtpServerName
        {
            get
            {

                string key = string.Empty;

                key = string.Format("{0}-Smtp", Settings.Environment());

                return ConfigurationManager.AppSettings[key];
            }
        }

        /// <summary>
        /// Gets Smtp port on the server to send the message to
        /// </summary>
        public static int SmtpServerPort
        {
            get
            {
                return Environment() == "production" ? 25 : 587;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to use ssl channel to send email or not
        /// </summary>
        public static bool EnableSsl
        {
            get
            {
                return Environment() == "production" ? false : true;
            }
        }

        /// <summary>
        /// The No-Reply email addres
        /// </summary>
        /// <returns>The email address</returns>
        public static string NoReplyMailAddress
        {
            get
            {
                string key = string.Format("{0}-Noreply", Environment());
                return ConfigurationManager.AppSettings[key];
            }
        }

        /// <summary>
        /// Gets Username to login with on the smtp server
        /// </summary>
        public static string SmtpUser
        {
            get
            {
                return Settings.NoReplyMailAddress;
            }
        }

        /// <summary>
        /// Email ID
        /// </summary>
        public static string EmailID
        {
            get
            {
                return ConfigurationManager.AppSettings[string.Format("{0}-Email", Environment())] as string;
            }
        }

        /// <summary>
        /// Gets Password to use to login to smtp server
        /// </summary>
        public static string SmtpPassword
        {
            get
            {
                //return "9ED9C476-E67B-4EB2-BB24-CD213A3673D8";
                string key = string.Format("{0}-SmtpPassword", Environment());
                return ConfigurationManager.AppSettings[key];
            }
        }

        /// <summary>
        /// Get or sets StartedEmail
        /// </summary>
        private static bool startedEmail = true;
        public static bool StartedEmail
        {
            get
            {
                return startedEmail;
            }
            set
            {
                startedEmail = value;
            }
        }
    }
}
