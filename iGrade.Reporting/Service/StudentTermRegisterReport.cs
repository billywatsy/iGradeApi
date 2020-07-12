using iGrade.Domain.Dto;
using iGrade.Reporting.Domain;
using iGrade.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iGrade.Reporting.Service
{
    public class StudentTermRegisterReport
    {
        private UowRepository _uofRepository;
        public StudentTermRegisterReport(UowRepository uowRepository)
        {
            _uofRepository = uowRepository;
        }

        public SchoolStudentTermRegister GetTermStudentTermRegister(Guid schooID, Guid termID, ref StringBuilder sbError)
        {
            var dbFlag = false;
            var term = _uofRepository.TermRepository.GetByID(termID, ref dbFlag);

            if (term == null)
            {
                sbError.Append("Term does not exist");
                return null;
            }
            var enrollmentList = _uofRepository.StudentTermRegisterRepository.GetListBySchoolIDAndTermID(schooID, termID, ref dbFlag);

            if (enrollmentList == null )
            {
                sbError.Append("No Data for term");
                return null;
            }

            SchoolStudentTermRegister schoolTermEnrollment = new SchoolStudentTermRegister();
            schoolTermEnrollment.Levels = new List<SchoolStudentTermRegisterLevel>();
            schoolTermEnrollment.Classes = new List<SchoolStudentTermRegisterClass>();

            schoolTermEnrollment.OveralSchoolAll = enrollmentList.Count();
            schoolTermEnrollment.OveralSchoolFemale = enrollmentList.Where(c => !c.IsMale)?.Count() ?? 0;
            schoolTermEnrollment.OveralSchoolMale = enrollmentList.Where(c => c.IsMale)?.Count() ?? 0;


            var uniqueLevel = enrollmentList.Select(c => c.LevelID).Distinct();

            foreach (var level in uniqueLevel)
            {
                var levelObj = enrollmentList.FirstOrDefault(c => c.LevelID == level);
                schoolTermEnrollment.Levels.Add(new SchoolStudentTermRegisterLevel()
                {
                    LevelName = levelObj?.LevelName ,
                    OveralSchoolAll = enrollmentList.Where(c => c.LevelID == level)?.Count() ?? 0,
                    OveralSchoolFemale = enrollmentList.Where(c => c.LevelID == level && !c.IsMale)?.Count() ?? 0,
                    OveralSchoolMale = enrollmentList.Where(c => c.LevelID == level && c.IsMale)?.Count() ?? 0
                });
            }

            var uniqueClass = enrollmentList.Select(c => c.ClassID).Distinct();

            foreach (var @class in uniqueClass)
            {
                var classObj = enrollmentList.FirstOrDefault(c => c.ClassID == @class);
                schoolTermEnrollment.Classes.Add(new SchoolStudentTermRegisterClass()
                {
                    ClassID = (Guid)classObj.ClassID,
                    LevelName = classObj?.LevelName , 
                    ClassName = classObj?.ClassName ,
                    OveralSchoolAll = enrollmentList.Where(c => c.ClassID == @class)?.Count() ?? 0,
                    OveralSchoolFemale = enrollmentList.Where(c => c.ClassID == @class && !c.IsMale)?.Count() ?? 0,
                    OveralSchoolMale = enrollmentList.Where(c => c.ClassID == @class && c.IsMale)?.Count() ?? 0
                });
            }

            return schoolTermEnrollment ;
        }
         


    }
}

