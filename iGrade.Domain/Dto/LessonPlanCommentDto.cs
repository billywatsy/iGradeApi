using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Domain.Dto
{
    public class LessonPlanCommentDto
    {
        public Guid? LessonPlanCommentId { get; set; }
        public string Comment { get; set; }
        public bool IsEdited { get; set; }
        public bool IsMyComment { get; set; } = false;
        public DateTime? IsDeleted { get; set; }
        public string TeacherFullname { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
