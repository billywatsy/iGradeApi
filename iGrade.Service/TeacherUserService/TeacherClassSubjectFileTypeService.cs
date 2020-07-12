using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class TeacherClassSubjectFileTypeService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public TeacherClassSubjectFileTypeService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public List<TeacherClassSubjectFileType> GetTeacherClassSubjectFileTypeBySchoolId()
        {
            bool dbFlag = false;
            var list = _uofRepository.TeacherClassSubjectFileTypeRepository.GetListBySchoolId(_user.SchoolID, ref dbFlag);
            return list;
        }


        public TeacherClassSubjectFileType GetTeacherClassSubjectFileType(Guid teacherClassSubjectFileTypeId)
        {
            bool dbFlag = false;
            var list = _uofRepository.TeacherClassSubjectFileTypeRepository.GetTeacherClassSubjectFileType(teacherClassSubjectFileTypeId, ref dbFlag);
            return list;
        }


        public TeacherClassSubjectFileType GetTeacherClassSubjectFileTypeByCode(string code)
        {
            bool dbFlag = false;
            var list = _uofRepository.TeacherClassSubjectFileTypeRepository.GetTeacherClassSubjectFileTypeByCode(code, _user.SchoolID ,ref dbFlag);
            return list;
        }

        public TeacherClassSubjectFileType Save(TeacherClassSubjectFileType teacherClassSubjectFileType , ref StringBuilder sbError)
        {
            bool dbFlag = false;

            if (teacherClassSubjectFileType.TeacherClassSubjectFileTypeId == null || teacherClassSubjectFileType.TeacherClassSubjectFileTypeId == Guid.Empty)
            {
                teacherClassSubjectFileType.SchoolId = _user.SchoolID;
            }

            if (string.IsNullOrEmpty(teacherClassSubjectFileType.Description))
            {
                sbError.Append("Description is required");
                return null;
            }
            if (string.IsNullOrEmpty(teacherClassSubjectFileType.Code))
            {
                sbError.Append("Cod is required");
                return null;
            }

            var list = _uofRepository.TeacherClassSubjectFileTypeRepository.GetListBySchoolId(_user.SchoolID, ref dbFlag);

            if(list.Count() > 100)
            {
                sbError.Append("You have reached maximum teacherClassSubjectFileTypes allowed");
                return null;
            }
            else
            {
                if (!string.IsNullOrEmpty(teacherClassSubjectFileType.TeacherClassSubjectFileTypeId.ToString()))
                {
                    var dbTeacherClassSubjectFileType = list.Where(c => c.TeacherClassSubjectFileTypeId == teacherClassSubjectFileType.TeacherClassSubjectFileTypeId).FirstOrDefault();
                    if (dbTeacherClassSubjectFileType == null)
                    {
                        sbError.Append("TeacherClassSubjectFileType does not exist for school");
                        return null;
                    }
                    else
                    {
                        teacherClassSubjectFileType.Code = dbTeacherClassSubjectFileType.Code;
                        teacherClassSubjectFileType.SchoolId = _user.SchoolID;
                    }
                }
                else
                {
                    teacherClassSubjectFileType.SchoolId = _user.SchoolID;
                }
            }
            var isSaved = _uofRepository.TeacherClassSubjectFileTypeRepository.Save(teacherClassSubjectFileType , _user.Username, ref dbFlag);

            return isSaved;
        }

        public bool Delete(Guid teacherClassSubjectFileTypeId, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var teacherClassSubjectFileType = _uofRepository.TeacherClassSubjectFileTypeRepository.GetTeacherClassSubjectFileType(teacherClassSubjectFileTypeId, ref dbFlag);

            if (teacherClassSubjectFileType == null)
            {
                sbError.Append("teacherClassSubjectFileType Does not Exist");
                return false;
            }

            if (teacherClassSubjectFileType.SchoolId != _user.SchoolID)
            {
                sbError.Append("Error teacherClassSubjectFileType does not exist for school");
                return false;
            }

            return _uofRepository.TeacherClassSubjectFileTypeRepository.Delete((Guid)teacherClassSubjectFileType.TeacherClassSubjectFileTypeId, _user.Username, ref dbFlag);
        }
    }
}
 