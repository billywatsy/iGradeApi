using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iGrade.Api.Controllers.TeacherUserApi.Model
{
    public class StudentFormUpsert
    {
        [JsonProperty("studentID")]
        public Guid? StudentID { get; set; }
        [JsonProperty("schoolID")]
        public Guid? SchoolID { get; set; }
        [JsonProperty("studentName")]
        public string StudentName { get; set; }
        [JsonProperty("studentMidName")]
        public string StudentMidName { get; set; }
        [JsonProperty("studentSurname")]
        public string StudentSurname { get; set; }
        [JsonProperty("regNumber")]
        public string RegNumber { get; set; }
        [JsonProperty("iDnational")]
        public string IDnational { get; set; }
        [JsonProperty("isMale")]
        public bool IsMale { get; set; }
        [JsonProperty("dOB")]
        public System.DateTime DOB { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("contactEmail")]
        public string ContactEmail { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
}
