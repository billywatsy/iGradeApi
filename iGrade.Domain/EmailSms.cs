namespace iGrade.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EmailSms
    {
        [JsonProperty("schoolID")]
        public Guid SchoolID { get; set; }
        [JsonProperty("smsUsername")]
        public string SmsUsername { get; set; }
        [JsonProperty("smsPassword")]
        public string SmsPassword { get; set; }
        [JsonProperty("smsKey")]
        public string SmsKey { get; set; }
        [JsonProperty("smsServiceProviderCode")]
        public string SmsServiceProviderCode { get; set; }
        [JsonProperty("emailUsername")]
        public string EmailUsername { get; set; }
        [JsonProperty("emailPassword")]
        public string EmailPassword { get; set; }
        [JsonProperty("emailDisplayName")]
        public string EmailDisplayName { get; set; }
        [JsonProperty("emailPort")]
        public int EmailPort { get; set; }

        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }

        public School School { get; set; } 
    }
}
