using iGrade.Core.TeacherUserService.Common;
using iGrade.Domain;
using iGrade.Domain.Form;
using iGrade.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.SystemAdminService
{
    public class SchoolService
    {

        private UowRepository _uofRepository; 
        public SchoolService(UowRepository uofRepository)
        {
            _uofRepository = uofRepository; 
        }

        public List<School> GetList()
        {
            bool dbBool = false;
            return _uofRepository.SchoolRepository.GetList(ref dbBool) ?? new List<School>();
        }

        public List<Subscription> GetListSubscription()
        {
            bool dbBool = false;
            return _uofRepository.SubscribeRepository.GetList(ref dbBool) ?? new List<Subscription>();
        }

        public bool Save(SchoolCreateNewFORM schoolCreateNewFORM , ref StringBuilder sbError)
        {
            var isSaved = false;
            var school = new School()
            {
                SchoolID = Guid.NewGuid(),
                IsSchoolActive = true,
                SchoolCode = Guid.NewGuid().ToString("N").Substring(0 , 5),
                SchoolName = schoolCreateNewFORM.SchoolName 
            };

            var term = new Term()
            {
                Year = DateTime.Now.Year,
                TermNumber = 1,
                StartDate = DateTime.Now.AddDays(-1) , 
                EndDate = DateTime.Today
            };

            var setting = new Setting()
            { 
                ExamMarkSubmissionClosingDate = DateTime.Today ,
                TestMarkSubmissionClosingDate = DateTime.Now ,
                TermLastDateUpdated = DateTime.Today ,
                MaximumTermPerYear = schoolCreateNewFORM.MaximumTermPerYear  
            };

            var teacher = new Teacher()
            {
                TeacherUsername = schoolCreateNewFORM.TeacherUsername ,
                TeacherEmail = schoolCreateNewFORM.TeacherEmail ,
                TeacherPhone = schoolCreateNewFORM.TeacherPhone
            };
            var password = Guid.NewGuid().ToString().Substring(0, 5);
            isSaved = _uofRepository.SchoolRepository.CreateNewSchool(school , term , setting , teacher , password , ref isSaved);

            if (isSaved)
            {
                var message = $"Hi  {teacher.TeacherFullname } <br/> <center><h3>Welcome aboard!</h3><center><br/> Your account has now been setup for <b>{school.SchoolName}</b> " +
                              $"<br>Username : {teacher.TeacherUsername} <br/> Password : " + password;
                var sendEmail = Email.SendEmail(teacher.TeacherEmail, " New Account ", message, ref sbError);
            }
            return isSaved;
        }

        public bool SchoolActivateOrDeactivateAccount(Guid schoolID , bool isActive)
        {
            var isOk = true;
            isOk = _uofRepository.SchoolRepository.SchoolActivateOrDeactivateAccount(schoolID, isActive);
            return isOk;
        }
    }
}
