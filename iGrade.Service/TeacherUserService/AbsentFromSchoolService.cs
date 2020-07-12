using iGrade.Domain;
using iGrade.Domain.Dto;
using iGrade.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class AbsentFromSchoolService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public AbsentFromSchoolService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public List<AbsentFromSchoolDto> GetListByStudentAndTermId(Guid studentTermRegisterID, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.AbsentFromSchoolRepository.GetListAbsentByStudentTermRegisterID(studentTermRegisterID, ref dbFlag);
            return list;
        }

        public List<AbsentFromSchoolDto> GetListByClassIdAndTermId(Guid classID, Guid termID, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.AbsentFromSchoolRepository.GetListAbsentByClassIDAndTermID(classID, termID, ref dbFlag);
            return list;
        }

        public bool Save(Guid studentTermRegisterID, DateTime dayAbsent , string reason , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var enrollment = _uofRepository.StudentTermRegisterRepository.GetByEnrollemtID(studentTermRegisterID, ref dbFlag);

            if (enrollment == null)
            {
                sbError.Append("Is not registered for this term ");
                return false;
            }


            var student = _uofRepository.StudentRepository.GetStudentById(enrollment.StudentID, ref dbFlag);

            if(student == null)
            {
                sbError.Append("Reg number does not  exist for school");
                return false;
            }

            Domain.AbsentFromSchool absent = new AbsentFromSchool()
            {
                DateCreated = DateTime.Now , 
                Reason = reason ,
                StudentTermRegisterID = (Guid)enrollment.StudentTermRegisterID,
                DayAbsent = dayAbsent
            };

            if (absent.DayAbsent > DateTime.Today)
            {
                sbError.Append("Date should be less than today");
            }

            var schoolSetting = _uofRepository.SettingRepository.GetSettingBySchoolID(_user.SchoolID, ref dbFlag);
            var school = _uofRepository.SchoolRepository.GetSchoolBySchoolID(_user.SchoolID, ref dbFlag);
            if (schoolSetting == null || school == null)
            {
                sbError.Append("Error failed getting school setting");
                return false;
            }
            

            var term = _uofRepository.TermRepository.GetByID(schoolSetting.TermID, ref dbFlag);

            if (term == null)
            {
                sbError.Append("Term not found");
            }
            else
            {
                if (absent.DayAbsent < term.StartDate || absent.DayAbsent > term.EndDate)
                {
                    sbError.Append("Absent date should be within term dates");
                }
            }

            if (!string.IsNullOrEmpty(sbError.ToString()))
            {
                return false;
            }

           var isSaved = _uofRepository.AbsentFromSchoolRepository.Save(absent,_user.Username , ref dbFlag);
           if (!isSaved)
            {
                sbError.Append("error saving absent");
                return false;
            }
            else
            {
                if (schoolSetting.AbsentFromSchoolEmailNotify)
                {
                    var gender = student.IsMale ? "He " : " She";
                    var message = $"Hi  <br/> This is a notification for student name {student.StudentName }  {student.StudentSurname } ." +
                        $"<br>{gender} was recorded absent on the  : {absent.DayAbsent.ToString("dd MMMM yyyy")} for reason [<i> {reason} </i>] .<br><center> <b> School Name :</b>{school.SchoolName}</center?";
                    if (!string.IsNullOrEmpty(student?.Email))
                    {
                        var sendEmail = iGrade.Core.TeacherUserService.Common.Email.SendEmail(student.Email, "  Absent Student ", message, ref sbError);
                    }

                }
            }

           /*
            var smsEmail = _uofRepository.EmailSmsRepository.GetEmailSmsByID(_user.SchoolID, ref dbFlag);

            if(smsEmail != null)
            {
                if (schoolSetting.AbsentFromSchoolSmsNotify)
                {
                    // send sms
                }

                if (schoolSetting.AbsentFromSchoolEmailNotify)
                {
                    string myLink = System.Configuration.ConfigurationManager.AppSettings["AppBaseUrl"].ToString();
                    var message = $"Hi  <br/> This is a notification for student name {student.StudentName }  {student.StudentSurname } ." +
                        $"<br> was recorded absent on the  : {absent.DayAbsent.ToString("dd MMMM yyyy")} for school name : <b>{school.SchoolName}</b>";
                    if (!string.IsNullOrEmpty(student?.Email))
                    {
                        var sendEmail = iGrade.Core.TeacherUserService.Common.Email.SendEmail(student.Email, "  Absent Student ", message, ref sbError);
                    }

                }
            }
            */
            return true;
        }

        public bool Delete(Guid absentId , ref StringBuilder sbError)
        {
            bool state = true;
            bool dbFlag = false;

            var absent = _uofRepository.AbsentFromSchoolRepository.GetByID(absentId, ref dbFlag);

            if(absent == null)
            {
                sbError.Append("Error failed getting record details");
                return false;
            }

             
            
            state = _uofRepository.AbsentFromSchoolRepository.Delete(absentId , _user.Username , ref dbFlag );
            if (dbFlag)
            {
                sbError.Append("Error deleting");
                return false;
            }
            return state;
        }
    }
}
 