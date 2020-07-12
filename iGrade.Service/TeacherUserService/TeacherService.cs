 
using iGrade.Domain;
using iGrade.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class TeacherService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public TeacherService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public Teacher Save(Teacher teacher , ref StringBuilder sbError)
        {
            if(teacher == null)
            {
                sbError.Append("Fill in required fields");
                return null;
            }

            if(!long.TryParse(teacher.TeacherPhone , out long phone))
            {
                sbError.Append("Enter valid phone number");
                return null;
            }

            bool dbFlag = false; 
            if (string.IsNullOrEmpty(teacher.TeacherID.ToString()))
            {
                teacher.SchoolID = _user.SchoolID;
            }
            else 
            {
                if (_user.SchoolID != teacher.SchoolID)
                {
                    sbError.Append("Student does not belong to school");
                    return teacher;
                }
                var school = _uofRepository.SchoolRepository.GetSchoolBySchoolID(_user.SchoolID, ref dbFlag);
                var numberOfTeacher = _uofRepository.TeacherRepository.CountTeachersBySchoolId(_user.SchoolID, ref dbFlag);
                if (numberOfTeacher > iGrade.Core.TeacherUserService.Common.PolicyCommon.OveralTeacher)
                {
                    sbError.Append("Error school has reached maximum of allowed teachers . ");
                    return null;
                }
                var dbTeacher = _uofRepository.TeacherRepository.GetTeacherById((Guid)teacher.TeacherID, ref dbFlag);
                if (dbTeacher == null) 
                {
                    return teacher;
                } 
                teacher.SchoolID = dbTeacher.SchoolID;
            }
            var ooneTimePin = Guid.NewGuid().ToString().Substring(0 , 4);
            var schoolAccount = _uofRepository.SchoolRepository.GetSchoolBySchoolID(teacher.SchoolID, ref dbFlag);
            if(schoolAccount == null)
            {
                sbError.Append("Failed getting school account");
                return null;
            }
            var isFirstTime = false;
            if(teacher.TeacherID == null || teacher.TeacherID == Guid.Empty)
            {
                isFirstTime = true;

                var isUsername = _uofRepository.TeacherRepository.IsUsernameExist(teacher.TeacherUsername, ref dbFlag);

                if (isUsername)
                {
                    sbError.Append("username already exist");
                    return null;
                }


                var isPhoneExist = _uofRepository.TeacherRepository.IsPhoneExist(teacher.TeacherPhone, ref dbFlag);

                if (isPhoneExist)
                {
                    sbError.Append("phone already exist");
                    return null;
                }

                var isEmailExist = _uofRepository.TeacherRepository.IsEmailExist(teacher.TeacherEmail, ref dbFlag);

                if (isEmailExist)
                {
                    sbError.Append("email already exist");
                    return null;
                }

            }
            teacher = _uofRepository.TeacherRepository.Save(teacher, ooneTimePin , _user.Username, ref dbFlag);

            if(teacher != null)
            {
                if (isFirstTime)
                {
                    string myLink = "";// System.Configuration.ConfigurationManager.AppSettings["AppBaseUrl"].ToString() ;
                    var message = $"Hi  {teacher.TeacherFullname }<br/> Your account has now been setup for <b>{schoolAccount.SchoolName}</b> .Click <a href='{myLink}'> here </a> to sign in or paste link to browser {myLink} "+
                        $"<br>Username : {teacher.TeacherUsername} <br/> Password : "+ ooneTimePin;
                    var sendEmail = iGrade.Core.TeacherUserService.Common.Email.SendEmail(teacher.TeacherEmail , " New Account ", message, ref sbError);

                }
            }
            return teacher;
        }

        public PagedList<TeacherDto> GetPagedListTeacherSearch(int PageSize, int PageNo, string searchValue, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            PagedList<TeacherDto> list = _uofRepository.TeacherRepository.GetPagedSearch(_user.SchoolID, PageSize, PageNo, searchValue, ref dbFlag);
            return list;
        }

        public List<Teacher> GetTeachersBySchoolId(  ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var teacherList = _uofRepository.TeacherRepository.GetListTeacherBySchoolId(_user.SchoolID, ref dbFlag);

            return teacherList;
        }
        public List<Teacher> GetTeachersBySchoolIdAndActiveStatus(bool IsActive, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var teacherList = _uofRepository.TeacherRepository.GetListTeacherBySchoolIdAndActiveStatus(_user.SchoolID, IsActive ,ref dbFlag);
            return teacherList;
        }

        public Teacher GetTeacherByTeacherId(Guid teacherId, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var teacher = _uofRepository.TeacherRepository.GetTeacherById(teacherId, ref dbFlag);
            return teacher;
        }

        public bool Delete(Guid teacherId, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var teacher = _uofRepository.TeacherRepository.GetTeacherById(teacherId, ref dbFlag);
            
            if (dbFlag)
            {
                sbError.Append("Error getting teacher details Does not Exist");
                return false;
            }
            if (teacher == null)
            {
                sbError.Append("Teacher Does not Exist");
                return false;
            }

            if (teacher.SchoolID != _user.SchoolID)
            {
                sbError.Append("Error teacher does not exist for school");
                return false;
            }

            if (teacher.TeacherID == _user.TeacherID)
            {
                sbError.Append("Error you can not delete yourself ");
                return false;
            }

            return _uofRepository.TeacherRepository.Delete((Guid)teacher.TeacherID, _user.Username, ref dbFlag);
        }
    }
}
 