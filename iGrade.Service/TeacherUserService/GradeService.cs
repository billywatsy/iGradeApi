using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iGrade.Domain.Dto;

namespace iGrade.Core.TeacherUserService
{
    public class GradeService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public GradeService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user;
        }

        public List<Grade> GetListBySchoolID(ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.GradeRepository.GetListBySchoolId(_user.SchoolID, ref dbFlag) ?? new List<Grade>();
            return list;
        }

        public Grade GetByID(Guid gradeID , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.GradeRepository.GetGradeById(gradeID, ref dbFlag);
            return list;
        }


        public List<GradeDto> GetListAndGradeMArksBySchoolID(ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.GradeRepository.GetListBySchoolId(_user.SchoolID, ref dbFlag) ?? new List<Grade>();
            List<GradeDto> final = new List<GradeDto>();

            foreach (var grade in list)
            {
                var gradeInGrade = (_uofRepository.GradeMarkRepository.GetGradeMarkListByGradeId((Guid)grade.GradeID, ref dbFlag)
                                    ?? new List<GradeMark>());


                List<GradeMarkDto> finalGrades = new List<GradeMarkDto>();

                foreach (var gradeMarkDto in gradeInGrade)
                {
                    finalGrades.Add(new GradeMarkDto()
                    {
                        GradeMarkID = (Guid)gradeMarkDto.GradeMarkID ,
                        GradeID = gradeMarkDto.GradeID,
                        FromMark = gradeMarkDto.FromMark,
                        ToMark = gradeMarkDto.ToMark,
                        GradeValue = gradeMarkDto.GradeValue
                    });
                }
                final.Add(new GradeDto()
                { 
                    Description = grade.Description,
                    GradeMarkList = finalGrades
                });
            }
            return final;
        }


        public Grade Save(Grade grade, ref StringBuilder sbError)
        {

            if (grade == null)
            {
                sbError.Append("enter all fields required");
                return null;
            }
            
            

            var dbFlag = false;

            var grades = _uofRepository.GradeRepository.GetListBySchoolId(_user.SchoolID, ref dbFlag) ?? new List<Grade>();

            if (grade.GradeID != null)
            {
                if (grades.Count() > 10)
                {
                    sbError.Append("You have reached maximum number of grades allowed");
                    return null;
                }
            }
            
                grade.SchoolID = _user.SchoolID;
            if (string.IsNullOrEmpty(grade.Description))
            {
                sbError.Append("Description is required");
                return null;
            }
             
            if (grade.Description.Length <= 1 || grade.Description.Length >= 50)
            {
                sbError.Append("Description should not be long than 50 characters");
                return null;
            }
            var save = _uofRepository.GradeRepository.Save(grade, _user.Username, ref dbFlag);
            return save;
            
        }

        public bool Delete(Guid gradeID , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var grades = _uofRepository.GradeMarkRepository.GetGradeMarkListByGradeId(gradeID, ref dbFlag);

            if(grades != null && grades.Count() >= 1)
            {
                sbError.Append("Some grade marks are dependant on it ");
                return false;
            }
            return _uofRepository.GradeRepository.Delete(gradeID, _user.Username, ref dbFlag);
        }
    }
}
