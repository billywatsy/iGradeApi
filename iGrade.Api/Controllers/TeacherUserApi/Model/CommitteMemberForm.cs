using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iGrade.Api.Controllers.TeacherUserApi.Model
{
    public class CommitteMemberForm
    {
        [JsonProperty("committeMemberID")]
        public Guid? CommitteMemberID { get; set; } 
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("fullname")]
        public string Fullname { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
