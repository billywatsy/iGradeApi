using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain
{
    public class Parent
    {
        [JsonProperty("parentEmail")]
        public string ParentEmail { get; set; }
        [JsonProperty("schoolCode")]
        public string SchoolCode { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; } 
        [JsonProperty("oneTimePin")]
        public string OneTimePin { get; set; } 
        [JsonProperty("webToken")]
        public string WebToken { get; set; }
        [JsonProperty("webTokenLastUpdated")]

        public DateTime WebTokenLastUpdated { get; set; }
        [JsonProperty("mobileToken")]

        public string MobileToken { get; set; }
        [JsonProperty("isAuthenticated")]
        public bool IsAuthenticated { get; set; }


        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }
    }
}
