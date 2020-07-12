
namespace iGrade.Domain.Dto
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    
    public class AbsentFromSchoolDto
    {
        [JsonProperty("absentFromSchoolID")]
        public Guid? AbsentFromSchoolID { get; set; }
        [JsonProperty("regNumber")]
        public string RegNumber { get; set; }
        [JsonProperty("studentName")]
        public string StudentName { get; set; }
        [JsonProperty("dayAbsent")]
        public System.DateTime DayAbsent { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; } 
    }
}
