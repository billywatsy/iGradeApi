using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iGrade.Api.Controllers.TeacherUserApi.Model
{
    public class DeleteClass
    {
        public Guid classId { get; set; }
        public Guid teacherId { get; set; } 
    }
}
