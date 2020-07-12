using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Domain.ParentDtos
{
    public class SchoolDataDto
    {
        public School School { get; set; }
        public SchoolInformation SchoolInformation { get; set; }
        public Term Term { get; set; }
    }
}
