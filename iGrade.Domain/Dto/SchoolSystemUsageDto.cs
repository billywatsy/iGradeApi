using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class SchoolSystemUsageDto
    {
        public Guid SchoolID { get; set; }
        public string SchoolName { get; set; }
        public long NumberOfStudents{ get; set; }
        public long NumberOfClasses{ get; set; }
        public long NumberOfSubjects{ get; set; }
        public long NumberOfTeachers{ get; set; }
        public long NumberOfExams{ get; set; }
        public long NumberOfTests { get; set; }
        public long NumberOf { get; set; }
    }
}
