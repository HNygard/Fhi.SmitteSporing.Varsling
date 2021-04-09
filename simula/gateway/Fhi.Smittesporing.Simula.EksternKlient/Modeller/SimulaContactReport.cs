using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Fhi.Smittesporing.Simula.EksternKlient.Modeller
{
    /// <summary>
    /// {
    ///   "phone_number": "+4798765432",
    ///   "found_in_system": false,
    ///   "last_activity": "2020-04-22T06:05:38.611319Z",
    ///   "contacts": [{
    ///     [phoneNumber: string]: SimulaContact
    ///   }]
    /// }
    /// </summary>
    public class SimulaContactReport
    {
        [JsonProperty(PropertyName = "phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "found_in_system")]
        public bool FoundInSystem { get; set; }

        [JsonProperty(PropertyName = "last_activity")]
        public DateTime? LastActivity { get; set; }

        [JsonProperty(PropertyName = "contacts")]
        public Dictionary<string, SimulaContact>[] Contacts { get; set; }
    }

    /// <summary>
    /// {
    ///   "pin_code": "pin_code"
    ///   "cumulativ": SimulaContactCumulativeInfo,
    ///   "daily": {
    ///     [date: string]: SimulaContactDailyInfo
    ///   },
    ///   "version_info": {
    ///     "pipeline": "",
    ///     "device": ""
    ///   }
    /// }
    /// </summary>
    public class SimulaContact
    {
        [JsonProperty(PropertyName = "pin_code")]
        public string PinCode { get; set; }

        [JsonProperty(PropertyName = "cumulative")]
        public SimulaContactCumulativeInfo Cumulative { get; set; }

        [JsonProperty(PropertyName = "daily")]
        public Dictionary<string, SimulaContactDailyInfo> Daily { get; set; }

        [JsonProperty(PropertyName = "version_info")]
        public SimulaContactVersionInfo VersionInfo { get; set; }
    }

    /// <summary>
    /// "version_info": {
    ///   "pipeline": "",
    ///   "device": ""
    /// }
    /// </summary>
    public class SimulaContactVersionInfo
    {
        [JsonProperty(PropertyName = "pipeline")]
        public string Pipeline { get; set; }
        [JsonProperty(PropertyName = "device")]
        public List<string> Device { get; set; }
    }

    /// <summary>
    /// {
    ///   "all_contacts": {
    ///     "risk_cat": "high",
    ///     "bar_plot": "png:base64",
    ///     "number_of_contacts": 1,
    ///     "days_in_contact": 1,
    ///     "points_of_interest": {
    ///       "outside": 485.950495049505,
    ///       "residential": 319.0
    ///     }
    ///   },
    ///   "gps_contacts": {
    ///     "cumulative_duration": 0,
    ///     "cumulative_risk_score": 0,
    ///     "days_in_contact": 1,
    ///   },
    ///   "bt_contacts": {
    ///     "cumulative_duration": 804.950495049505,
    ///     "cumulative_risk_score": 13.415841584158416,
    ///     "days_in_contact": 1,
    ///   }
    /// }
    /// </summary>
    public class SimulaContactCumulativeInfo
    {
        [JsonProperty(PropertyName = "all_contacts")]
        public All AllContacts { get; set; }
        [JsonProperty(PropertyName = "bt_contacts")]
        public BtInfo BtContacts { get; set; }
        [JsonProperty(PropertyName = "gps_contacts")]
        public GpsInfo GpsContacts { get; set; }

        public class All
        {
            [JsonProperty(PropertyName = "risk_cat")]
            public string RiskCat { get; set; }

            [JsonProperty(PropertyName = "bar_plot")]
            public string BarPlot { get; set; }

            [JsonProperty(PropertyName = "number_of_contacts")]
            public int NumberOfContacts { get; set; }

            [JsonProperty(PropertyName = "days_in_contact")]
            public int DaysInContact { get; set; }

            [JsonProperty(PropertyName = "points_of_interest")]
            public Dictionary<string, double> PointsOfInterest { get; set; }
        }

        public class PerType
        {
            [JsonProperty(PropertyName = "cumulative_duration")]
            public double CumulativeDuration { get; set; }

            [JsonProperty(PropertyName = "cumulative_risk_score")]
            public double CumulativeRiskScore { get; set; }

            [JsonProperty(PropertyName = "days_in_contact")]
            public int DaysInContact { get; set; }
        }

        /// <summary>
        /// PerType in addion to the following:
        /// "hist_plot": "-- base 64 encoded image --"
        /// </summary>
        public class GpsInfo : PerType
        {
            [JsonProperty(PropertyName = "hist_plot")]
            public string HistPlot { get; set; }
        }

        /// <summary>
        /// PerType in addion to the following:
        /// "bt_very_close_duration": 804.950495049505,
        /// "bt_relatively_close_duration": 0.0,
        /// "bt_close_duration": 0.0
        /// </summary>
        public class BtInfo : PerType
        {
            [JsonProperty(PropertyName = "bt_very_close_duration")]
            public double BtVeryCloseDuration { get; set; }

            [JsonProperty(PropertyName = "bt_relatively_close_duration")]
            public double BtRelativelyCloseDuration { get; set; }

            [JsonProperty(PropertyName = "bt_close_duration")]
            public double BtCloseDuration { get; set; }
        }
    }

    /// <summary>
    /// {
    ///   "all_contacts": {
    ///     "summary_plot": "<div>Here be plot</div>",
    ///     "points_of_interest": {
    ///         "outside": 485.950495049505,
    ///         "residential": 319.0
    ///     }
    ///   },
    ///   "bt_contacts": {
    ///     "number_of_contacts": 1,
    ///     "cumulative_duration": 804.950495049505,
    ///     "median_distance": 1.0
    ///   },
    ///   "gps_contacts": {
    ///     "number_of_contacts": 0,
    ///     "cumulative_duration": 0,
    ///     "median_distance": null
    ///   }
    /// }
    /// </summary>
    public class SimulaContactDailyInfo
    {
        [JsonProperty(PropertyName = "all_contacts")]
        public All AllContacts { get; set; }
        [JsonProperty(PropertyName = "bt_contacts")]
        public BtInfo BtContacts { get; set; }
        [JsonProperty(PropertyName = "gps_contacts")]
        public PerType GpsContacts { get; set; }

        public class All
        {
            [JsonProperty(PropertyName = "summary_plot")]
            public string SummaryPlot { get; set; }

            [JsonProperty(PropertyName = "points_of_interest")]
            public Dictionary<string, double> PointsOfInterest { get; set; }
        }

        public class PerType
        {
            [JsonProperty(PropertyName = "cumulative_duration")]
            public double CumulativeDuration { get; set; }

            [JsonProperty(PropertyName = "cumulative_risk_score")]
            public double CumulativeRiskScore { get; set; }

            [JsonProperty(PropertyName = "median_distance")]
            public double? MedianDistance { get; set; }
        }

        /// <summary>
        /// PerType in addion to the following:
        /// "bt_very_close_duration": 804.950495049505,
        /// "bt_relatively_close_duration": 0.0,
        /// "bt_close_duration": 0.0
        /// </summary>
        public class BtInfo : PerType
        {
            [JsonProperty(PropertyName = "bt_very_close_duration")]
            public double BtVeryCloseDuration { get; set; }

            [JsonProperty(PropertyName = "bt_relatively_close_duration")]
            public double BtRelativelyCloseDuration { get; set; }

            [JsonProperty(PropertyName = "bt_close_duration")]
            public double BtCloseDuration { get; set; }
        }
    }
}