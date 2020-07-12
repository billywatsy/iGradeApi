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
    public class TeacherClassSubjectFileService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public TeacherClassSubjectFileService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public TeacherClassSubjectFileDto GetTeacherClassSubjectFile(Guid teacherClassSubjectFileId)
        {
            bool dbFlag = false;
            var list = _uofRepository.TeacherClassSubjectFileRepository.GetTeacherClassSubjectFile(teacherClassSubjectFileId, ref dbFlag);
            return list;
        }
        
        public List<TeacherClassSubjectFileDto> GetTeacherClassSubjectFilesByTeacherClassSubjectIdAndTeacherClassSubjectFileTypeId(Guid TeacherClassSubjectId , Guid TeacherClassSubjectFileTypeId)
        {
            bool dbFlag = false;
            var list = _uofRepository.TeacherClassSubjectFileRepository.GetListByTeacherClassSubjectIdAndTeacherClassSubjectFileTypeId(TeacherClassSubjectId, TeacherClassSubjectFileTypeId, ref dbFlag);
            return list;
        }

        public TeacherClassSubjectFile PreSaveValidate(Guid TeacherClassSubjectID,
                                                      Guid TeacherClassSubjectFileTypeID ,
                                                      string Description ,
                                                      string Title ,
                                                      ref string schoolCode ,
                                                      ref StringBuilder sbError)
        {

            bool dbFlag = false;
            if (Guid.Empty == TeacherClassSubjectFileTypeID || TeacherClassSubjectFileTypeID == null)
            {
                sbError.Append("File type is required");
            }
            else
            {
                var fileType = _uofRepository.TeacherClassSubjectFileTypeRepository.GetTeacherClassSubjectFileType(TeacherClassSubjectFileTypeID, ref dbFlag);

                if (fileType == null)
                {
                    sbError.Append("File type does not exist for school");
                }
            }

            if (Guid.Empty == TeacherClassSubjectID || TeacherClassSubjectID == null)
            {
                sbError.Append("Failed to get subject class ");
            }
            else
            {
                var tcs = _uofRepository.TeacherClassSubjectRepository.GetTeacherClassSubjectByID(TeacherClassSubjectID, ref dbFlag);

                if (tcs == null)
                {
                    sbError.Append("Teacher class subject does not exist");
                }
            }
            
            if(string.IsNullOrEmpty(Description) || Description?.Length >= 200)
            {
                sbError.Append("Description should be less than 200 characters");
            }

            if(string.IsNullOrEmpty(Title) || Title?.Length >= 100)
            {
                sbError.Append("Title should be less than 200 characters");
            }

            var school = _uofRepository.SchoolRepository.GetSchoolBySchoolID(_user.SchoolID, ref dbFlag);

            if (school == null)
            {
                sbError.Append("Faild getting school");
            }

            schoolCode = school.SchoolCode;

            var teacherClassSubjct = new TeacherClassSubjectFile();
            teacherClassSubjct.Description = Description;
            teacherClassSubjct.Title = Title;
            teacherClassSubjct.TeacherClassSubjectId = TeacherClassSubjectID;
            teacherClassSubjct.TeacherClassSubjectFileTypeId = TeacherClassSubjectFileTypeID;
            return teacherClassSubjct;
        }

        public TeacherClassSubjectFile Save(TeacherClassSubjectFile teacherClassSubjectFile , ref StringBuilder sbError)
        {
            bool dbFlag = false;

            if (teacherClassSubjectFile == null )
            {
                sbError.Append(" please fill in all fields");
                return null;
            }

            
            var isSaved = _uofRepository.TeacherClassSubjectFileRepository.Insert(teacherClassSubjectFile , _user.Username, ref dbFlag);

            return isSaved;
        }

        public bool Delete(Guid teacherClassSubjectFileId, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var teacherClassSubjectFile = _uofRepository.TeacherClassSubjectFileRepository.GetTeacherClassSubjectFile(teacherClassSubjectFileId, ref dbFlag);

            if (teacherClassSubjectFile == null)
            {
                sbError.Append("teacherClassSubjectFile Does not Exist");
                return false;
            }

            return _uofRepository.TeacherClassSubjectFileRepository.Delete((Guid)teacherClassSubjectFile.TeacherClassSubjectFileId, _user.Username, ref dbFlag);
        }
    }
}
 