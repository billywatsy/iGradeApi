using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Domain
{
    public class LessonPlanComment
    {
        [JsonProperty("lessonPlanCommentId")]
        public Guid? LessonPlanCommentId { get; set; }
        [JsonProperty("lessonPlanId")]
        public Guid LessonPlanId { get; set; }
        [JsonProperty("teacherId")]
        public Guid TeacherId { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
        [JsonProperty("lastModifiedDate")]
        public System.DateTime LastModifiedDate { get; set; }
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
