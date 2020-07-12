using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class StudentTermRegisterDto
    {
        public Guid StudentTermRegisterID { get; set; }
        public Guid StudentID { get; set; }
        public Guid ClassID { get; set; }
        public Guid LevelID { get; set; }
        public Guid TermID { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string StudentName { get; set; }
        public string RegNumber { get; set; }
        public string IdNumber { get; set; }
        public string ClassName { get; set; }
        public string LevelName { get; set; }
        public int TermYear { get; set; }
        public int  TermNumber { get; set; }
        public bool  IsMale { get; set; } 
    }
}
