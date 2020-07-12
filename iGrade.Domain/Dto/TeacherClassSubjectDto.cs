using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class TeacherClassSubjectDto
    {
        [JsonProperty("teacherClassSubjectID")]
        public Guid TeacherClassSubjectID { get; set; }
        [JsonProperty("classID")]
        public Guid ClassID { get; set; }
        [JsonProperty("subjectID")]
        public Guid SubjectID { get; set; }
        [JsonProperty("teacherID")]
        public Guid TeacherID { get; set; }
        [JsonProperty("levelID")]
        public Guid LevelID { get; set; }
        [JsonProperty("schoolID")]
        public Guid SchoolID { get; set; }
        [JsonProperty("termID")]
        public Guid TermID { get; set; }

        [JsonProperty("levelName")]
        public string LevelName { get; set; }
        [JsonProperty("subjectCode")]
        public string SubjectCode { get; set; }
        [JsonProperty("subjectName")]
        public string SubjectName { get; set; }
        [JsonProperty("className")]
        public string ClassName { get; set; }
        [JsonProperty("teacherName")]
        public string TeacherName { get; set; }
        [JsonProperty("gradeID")]
        public Guid GradeID { get; set; }
    }


}
