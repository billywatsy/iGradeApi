namespace iGrade.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class TeacherClassSubjectFileType
    {
        [JsonProperty("teacherClassSubjectFileTypeId")]
        public Guid? TeacherClassSubjectFileTypeId { get; set; }
        [JsonProperty("schoolId")]
        public Guid SchoolId { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("canParentAccessFile")]
        public bool CanParentAccessFile { get; set; }
        [JsonProperty("canStudentAccessFile")]
        public bool CanStudentAccessFile { get; set; }


        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }
         
    }
}
