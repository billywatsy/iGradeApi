using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class StudentProfile
    {
        public string RegNumber { get; set; } 
        public string IdNumber { get; set; } 
        public string Name { get; set; } 
        public string MidName { get; set; } 
        public string Surname { get; set; } 
        public DateTime DateOfBirth { get; set; } 
        public string SchoolLogo { get; set; }
        public bool IsMale { get; set; }
    }
}
