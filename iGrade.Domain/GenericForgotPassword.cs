using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain
{
    public class GenericForgotPassword
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("date")]
        public DateTime Date { get; set; }


        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }
        /*
          ADMIN , TEACHER, PARENT
         */
        public string UserType { get; set; }
    }
}
