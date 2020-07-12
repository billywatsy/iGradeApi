using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Reporting.Domain
{
    public class SchoolStudentTermRegister
    {
        public int OveralSchoolAll { get; set; }
        public int OveralSchoolMale { get; set; }
        public int OveralSchoolFemale { get; set; }
        public List<SchoolStudentTermRegisterLevel> Levels { get; set; }
        public List<SchoolStudentTermRegisterClass> Classes { get; set; }
    }
    public class SchoolStudentTermRegisterLevel
    {
        public string LevelName { get; set; }
        public int OveralSchoolAll { get; set; }
        public int OveralSchoolMale { get; set; }
        public int OveralSchoolFemale { get; set; }
    }
    public class SchoolStudentTermRegisterClass
    {
        public Guid ClassID { get; set; }
        public string ClassName { get; set; }
        public string LevelName { get; set; }
        public int OveralSchoolAll { get; set; }
        public int OveralSchoolMale { get; set; }
        public int OveralSchoolFemale { get; set; }
    }
}
