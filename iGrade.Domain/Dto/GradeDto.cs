using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class GradeDto
    {
        public Guid SchoolID { get; set; } 
        public string Description { get; set; }

        public List<GradeMarkDto> GradeMarkList { get; set; }
    }
}
