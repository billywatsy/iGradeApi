namespace iGrade.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class TeacherClassSubjectFile
    {
        [JsonProperty("teacherClassSubjectFileId")]
        public Guid? TeacherClassSubjectFileId { get; set; }
        [JsonProperty("teacherClassSubjectId")]
        public Guid TeacherClassSubjectId { get; set; }
        [JsonProperty("teacherClassSubjectFileTypeId")]
        public Guid TeacherClassSubjectFileTypeId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("fileSizeInBytes")]
        public double FileSizeInBytes { get; set; }
        [JsonProperty("filename")]
        public string Filename { get; set; } 
        
        [JsonProperty("fullurl")]
        public string FullUrl { get; set; }
        [JsonProperty("createdDate")]
        public System.DateTime CreatedDate { get; set; }

        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }
         
    }
}
