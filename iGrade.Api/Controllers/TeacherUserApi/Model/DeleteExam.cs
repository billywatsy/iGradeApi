using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iGrade.Api.Controllers.TeacherUserApi.Model
{
    public class DeleteExam
    { 
        public Guid studentTermRegisterID { get; set; }
        public Guid teacherClassSubjectID { get; set; } 
    }
}
