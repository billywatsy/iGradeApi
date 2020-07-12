using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class LogDto
    { 
        public string Username { get; set; }
        public string TeacherName { get; set; }
        public string EntityType { get; set; }
        public string ActionType { get; set; }
        public string ActionDetails { get; set; }
        public System.DateTime ActionDate { get; set; }
    }
}
