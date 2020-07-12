using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iGrade.Api.Controllers.TeacherUserApi.Model
{
    public class DeleteTestMark
    { 
        public Guid studentTermRegisterID { get; set; }
        public Guid testId { get; set; } 
    }
}
