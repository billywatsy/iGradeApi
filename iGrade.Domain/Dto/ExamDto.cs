using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class ExamDto
    {
        public Guid StudentTermRegisterID { get; set; }
        public Guid StudentID { get; set; }
        public Guid TeacherClassSubjectID { get; set; }  
        public Guid SchoolID { get; set; }
        public Guid TermID { get; set; }
        public int Year { get; set; }
        public int TermNumber { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string RegNumber { get; set; }
        public string FullName { get; set; }
        public byte Mark { get; set; }
        public string Grade { get; set; }
        public string Comment { get; set; }
        public bool IsMale { get; set; } 
        public string LevelName { get; set; } 
        public string ClassName { get; set; }
        public DateTime EndOfTermDate { get; set; } = DateTime.Now;
    }
}
