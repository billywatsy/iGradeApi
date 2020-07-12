namespace iGrade.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Class
    {
        [JsonProperty("classID")]
        public Guid? ClassID { get; set; }
        [JsonProperty("className")]
        public string ClassName { get; set; }
        [JsonProperty("levelID")]
        public Guid LevelID { get; set; }
        [JsonProperty("schoolID")]
        public Guid SchoolID { get; set; } 
        [JsonProperty("gradeID")]
        public Guid GradeID { get; set; }
        [JsonProperty("classCode")]
        public string ClassCode { get; set; }



        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Grade Grade { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Level Level { get; set; }
        
    }
}
