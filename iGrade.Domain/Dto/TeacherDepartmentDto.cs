using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Domain.Dto
{
    public class TeacherDepartmentDto
    {
        public Guid TeacherDepartmentId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherUsername { get; set; }
        public string TeacherFullname { get; set; }
        public bool IsHeadOfDepartment { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
    }
}
