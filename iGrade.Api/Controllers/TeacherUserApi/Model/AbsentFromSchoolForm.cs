using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iGrade.Api.Controllers.TeacherUserApi.Model
{
    public class AbsentFromSchoolForm
    {
        public Guid StudentTermRegisterID { get; set; }
        public DateTime DayAbsent { get; set; }
        public string Reason { get; set; }
    }
}
