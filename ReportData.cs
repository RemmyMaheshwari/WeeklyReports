using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeeklyReports
{
    class ReportData
    {
        public string portal { get; set; }
        public string user_region { get; set; }
        public string search_date { get; set; }
        public string year { get; set; }
        public string week_of_year { get; set; }
        public string keyword { get; set; }
        public string ip_owner { get; set; }
        public string product { get; set; }
        public string page_number { get; set; }
        public string status { get; set; }
        public string min_position_authorized { get; set; }
        public string min_position_unauthorized { get; set; }
        public string num_authorized { get; set; }
        public string num_unauthorized { get; set; }
        public string num_results { get; set; }
        public string perc_unauthorized { get; set; }
        public string avg_age_unauthorized_days { get; set; }
        public string keyword_template { get; set; }
        public string position_gap { get; set; }
        public string traffic_share { get; set; }
        public string unauthorized_traffic_share { get; set; }
        public string authorized_traffic_share { get; set; }
        public string used_historical_traffic_data { get; set; }
        public string sent_at { get; set; }
        public string number_reported { get; set; }
        public string number_removed { get; set; }
        public string average_time_to_action_minutes { get; set; }
        public string average_time_to_remove_minutes { get; set; }
        public string host_name  { get; set; }
        public string sum_time_to_action_Minutes { get; set; }
        public string sum_time_to_remove_minutes{ get; set; }
        public string first_unauthorized_search_date { get; set; }
        public string first_unauthorized_week_of_year{ get; set; }
        public string first_unauthorized_year{ get; set; }
        public string sum_of_numunauthorized{ get; set; }
    }
}
