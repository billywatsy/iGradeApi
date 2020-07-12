using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class StudentTermReviewDto
    {
        public Guid? StudentTermReviewID { get; set; }
        public Guid StudentTermRegisterID { get; set; }
        public Guid TeacherID { get; set; }
        public bool IsReviewGood { get; set; } 
        public string Body { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Star5 { get; set; }
        public string TeacherFullname { get; set; }
        public string RegNumber { get; set; }
        public string StudentFullname { get; set; }
        public string TermYear { get; set; }
        public string TermNumber { get; set; }
    }
}
