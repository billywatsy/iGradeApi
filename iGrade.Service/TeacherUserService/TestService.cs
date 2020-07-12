 
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
    public class TestService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public TestService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

       
        public List<TestDto> GetListTestDtoByTeacherClassSubjectID(Guid teacherClassSubjectID , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.TestRepository.GetListDtoTestByTeacherClassSubjectId(teacherClassSubjectID  , ref dbFlag);
            if (dbFlag)
            {
                sbError.Append("Database error");
            }

            return list;
        }

        public Test GetTestByTestId(Guid testID, ref StringBuilder sbError)
        {
            bool dbFlag = false;

            return _uofRepository.TestRepository.GetTestByTestId(testID, ref dbFlag);
        }

      
        public List<TestMarkDto> GetTestMarkListByTestId(Guid testID, ref StringBuilder sbError)
        {
            bool dbFlag = false;

            var test = _uofRepository.TestRepository.GetTestByTestId(testID, ref dbFlag);

            if (test != null)
            { 
                var list =  _uofRepository.TestMarkRepository.GetListTestMarksByTestId(testID, ref dbFlag);
                return list;
            }
            return null;
        }
        
        public bool Save(Test test , ref StringBuilder sbError)
        {
            var isClosed = IsTestSubmissionClosed(ref sbError);

            if (isClosed)
            {
                return false;
            }
            bool dbFlag = false;

            var teacherclasssubject = _uofRepository.TeacherClassSubjectRepository.GetTeacherClassSubjectByID(test.TeacherClassSubjectID, ref dbFlag);

            if(teacherclasssubject == null)
            {
                sbError.Append("teacher class subject does not exist");
                return false;
            }

            if(teacherclasssubject.TeacherID != _user.TeacherID)
            {
                sbError.Append("you are not the teacher for the class subject");
                return false;
            }


            var list = _uofRepository.TestRepository.GetListDtoTestByTeacherClassSubjectId(test.TeacherClassSubjectID, ref dbFlag);

            if (dbFlag)
            {
                sbError.Append("Error retrieving information");
                return false;
            }

            if (string.IsNullOrEmpty(test.TestID.ToString()) || Guid.Empty == test.TestID)
            {
                if (list.Where(c => c.TestDateCreated == DateTime.Today).FirstOrDefault() != null)
                {
                    sbError.Append("You can only create one test per day");
                    return false;
                }
                
                var testsPerMonth = list.Where(c => c.TestDateCreated.Year == DateTime.Today.Year)
                                        .Where(c => c.TestDateCreated.Month == DateTime.Today.Month)
                                        .ToList();
                if (testsPerMonth.Count() > 20)
                {
                    sbError.Append("You can only submit not more than 20 in a month");
                    return false;
                }

                var testsPerDay = list.Where(c => c.TestDateCreated.Year == DateTime.Today.Year)
                                        .Where(c => c.TestDateCreated.Month == DateTime.Today.Month)
                                        .Where(c => c.TestDateCreated.Day == DateTime.Today.Day)
                                        .ToList();
                if (testsPerDay.Count() >= 1)
                {
                    sbError.Append("One test for a particular date is allowed  please select another date");
                    return false;
                }
            }
            
           var e = _uofRepository.TestRepository.Save(test, _user.Username, ref dbFlag);
           if (e == null)
            {
                return false;
            }
            return true;
        }

        public bool Delete(Guid id, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var test = _uofRepository.TestRepository.GetTestByTestId(id, ref dbFlag);
            if (test == null)
            {
                sbError.Append("No data found");
                return false;
            }

            var isClosed = IsTestSubmissionClosed(ref sbError);

            if (isClosed)
            {
                return false;
            }

            var testMarkList = _uofRepository.TestMarkRepository.GetListTestMarksByTestId((Guid)test.TestID , ref dbFlag);
            if(testMarkList != null)
            {
                if (testMarkList.Count() > 0)
                {
                    sbError.Append("There is data for that test , record can not be deleted");
                    return false;
                }
            }

            return _uofRepository.TestRepository.Delete(test, _user.Username, ref dbFlag);
        }

        public bool IsTestSubmissionClosed(ref StringBuilder sbError)
        {
            var dbFlag = false;
            var setting = _uofRepository.SettingRepository.GetSettingBySchoolID(_user.SchoolID, ref dbFlag);

            if (setting == null)
            {
                sbError.Append("Failed getting school setting");
                return true;
            }

            if (setting.TestMarkSubmissionClosingDate < DateTime.UtcNow)
            {
                sbError.Append("Date has been closed for subim getting school setting");
                return true;
            }
            return false;
        }
    }
}
 