using Avo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Domain.Dto
{
    public class TeacherClassSubjectFileDto
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
        public string Fullurl { get; set; }

        [JsonProperty("filetype")]
        public string Filetype { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("subjectname")]
        public string Subjectname { get; set; }

        [JsonProperty("humanSize")]
        public string humanSize { get { return HelperFile.ToHumanSize(this.FileSizeInBytes);  } }
    }
}
