using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Domain.Dto
{
    public class ParentSessionDto
    {
        public string Token { get; set; }
        public string SchoolCode { get; set; }
        public string Email { get; set; }
    }
}
