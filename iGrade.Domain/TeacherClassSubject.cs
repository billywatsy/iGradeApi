
namespace iGrade.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    
    public class TeacherClassSubject
    {
        [JsonProperty("teacherClassSubjectID")]
        public Guid? TeacherClassSubjectID { get; set; }
        [JsonProperty("teacherID")]
        public Guid TeacherID { get; set; }
        [JsonProperty("subjectID")]
        public Guid SubjectID { get; set; }
        [JsonProperty("classID")]
        public Guid ClassID { get; set; }
        [JsonProperty("termID")]
        public Guid TermID { get; set; }

        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public  Class Class { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public  Subject Subject { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public  Teacher Teacher { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Term Term { get; set; }
         
    }
}
