using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Domain.Dto
{
    public class LessonPlanDto
    {
        [JsonProperty("lessonPlanId")]
        public Guid? LessonPlanId { get; set; }
        [JsonProperty("teacherClassSubjectId")]
        public Guid TeacherClassSubjectId { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("dateStart")]
        public DateTime DateStart { get; set; }
        [JsonProperty("dateEnd")]
        public DateTime DateEnd { get; set; }
        [JsonProperty("afterLessonComment")]
        public string AfterLessonComment { get; set; }
        [JsonProperty("approvedByID")]
        public Guid? ApprovedByID { get; set; }
        [JsonProperty("approvedByName")]
        public string ApprovedByName { get; set; }
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
