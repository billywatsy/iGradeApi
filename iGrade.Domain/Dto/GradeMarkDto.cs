using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class GradeMarkDto
    {
        public Guid GradeMarkID { get; set; }
        public Guid GradeID { get; set; } 
        public int FromMark { get; set; }
        public int ToMark { get; set; }
        public string GradeValue { get; set; }
    }
}
