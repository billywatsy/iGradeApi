//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iGrade.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    
    public class Test
    {
        [JsonProperty("testID")]
        public Guid? TestID { get; set; }
        [JsonProperty("teacherClassSubjectID")]
        public Guid TeacherClassSubjectID { get; set; }
        [JsonProperty("testTitle")]
        public string TestTitle { get; set; }
        [JsonProperty("outOf")]
        public byte OutOf { get; set; }
        [JsonProperty("testDateCreated")]
        public System.DateTime TestDateCreated { get; set; }

        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TeacherClassSubject TeacherClassSubject { get; set; }
    }
}