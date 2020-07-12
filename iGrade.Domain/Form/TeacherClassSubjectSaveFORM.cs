using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Form
{
    public class TeacherClassSubjectSaveMethodSubjectListFORM 
    {
        public Guid TeacherID { get; set; }
        public Guid ClassID { get; set; }
        public List<Guid> SubjectList { get; set; }
    }
    public class TeacherClassSubjectSaveMethodClassListFORM
    {
        public Guid TeacherID { get; set; }
        public Guid SubjectID { get; set; }
        public List<Guid> ClassList { get; set; }
    }
}
