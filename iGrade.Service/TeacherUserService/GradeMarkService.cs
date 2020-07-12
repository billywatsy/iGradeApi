using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class GradeMarkService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public GradeMarkService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user;
        }

        public List<GradeMark> GetListByGradeID(Guid gradeId , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.GradeMarkRepository.GetGradeMarkListByGradeId(gradeId, ref dbFlag);
            return list;
        }

        public GradeMark GetByID(Guid gradeMarkID , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.GradeMarkRepository.GetGradeMarkById(gradeMarkID, ref dbFlag);
            return list;
        }


        public GradeMark Save(GradeMark gradeMark, ref StringBuilder sbError)
        {
            var dbFlag = false;

            var gradeDescription = _uofRepository.GradeRepository.GetGradeById(gradeMark.GradeID, ref dbFlag);
            var grades = _uofRepository.GradeMarkRepository.GetGradeMarkListByGradeId(gradeMark.GradeID, ref dbFlag) ;

            if(gradeDescription == null)
            {
                sbError.Append("Grade Mark does not exist");
                return null;
            }

            if(gradeMark.GradeMarkID == null)
            {
                if(grades.Count() > 12)
                {
                    sbError.Append("Grade Mark should not be more than 12");
                    return null;
                }
            }

            if (gradeMark.FromMark >= gradeMark.ToMark)
            {
                sbError.Append("From mark should be less than To mark");
                return null;
            }
            if (gradeMark.FromMark < 0)
            {
                sbError.Append("From mark should be greater than zero");
                return null;
            }
            if (gradeMark.ToMark > 100)
            {
                sbError.Append("To mark should be less than 100");
                return null;
            }
            if (string.IsNullOrEmpty(gradeMark.GradeValue))
            {
                sbError.Append("Grade Default Percentage Value is required");
                return null;
            }
            if (gradeMark.GradeValue.Length == 1 || gradeMark.GradeValue.Length == 2)
            {
            }
            else
            {
                sbError.Append("Default value shoud not be 1 or 2 characters");
                return null;
            }
            var save = _uofRepository.GradeMarkRepository.Save(gradeMark, _user.Username, ref dbFlag);
            return save;
        }

        public bool Delete(Guid gradeMarkID , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var grade = _uofRepository.GradeMarkRepository.GetGradeMarkById(gradeMarkID, ref dbFlag);

            if(grade == null)
            {
                sbError.Append("grade marks does not exist ");
                return false;
            }
            return _uofRepository.GradeMarkRepository.Delete(gradeMarkID, _user.Username, ref dbFlag);
        }
    }
}
