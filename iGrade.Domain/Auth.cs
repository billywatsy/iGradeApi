using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace iGrade.Domain
{
    using Newtonsoft.Json;
    public class Auth
    {
        [JsonProperty("teacherID")]
        public Guid TeacherID { get; set; }
        [JsonProperty("forgotPassowrdCode")]
        public string ForgotPassowrdCode  { get; set; }
        [JsonProperty("isExpired")]
        public bool isExpired { get; set; }
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
        [JsonProperty("webToken")]
        public string WebToken { get; set; }
        [JsonProperty("lastLogin")]
        public string LastLogin { get; set; }
        [JsonProperty("oneTimePin")]
        public string OneTimePin { get; set; }

        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }
    }
}
