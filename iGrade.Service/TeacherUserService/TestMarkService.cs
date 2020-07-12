using iGrade.Repository;
using iGrade.Domain;
using iGrade.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class TestMarkService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public TestMarkService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public List<TestMarkDto> GetListTestMarksByStudentID(Guid studentID, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            return _uofRepository.TestMarkRepository.GetListTestMarksByStudentID(studentID, ref dbFlag) ?? new List<TestMarkDto>();
        }

        public List<TestMarkDto> GetListTestMarksByStudentIDAndTermID(Guid studentID, Guid termId , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            return _uofRepository.TestMarkRepository.GetListTestMarksByStudentIDAndTermID(studentID, termId , ref dbFlag) ?? new List<TestMarkDto>();
        }

        public List<TestMarkDto> GetTestMarkListByTestId(Guid testID, ref StringBuilder sbError)
        {
            bool dbFlag = false;

            var test = _uofRepository.TestRepository.GetTestByTestId(testID, ref dbFlag);

            if (test != null)
            {

                return _uofRepository.TestMarkRepository.GetListTestMarksByTestId(testID, ref dbFlag);
            }
            return null;
        }


        

        public int SaveBulkByRegNumber(List<iGrade.Domain.Form.TestMarkSaveFORM> testMarkList, Guid testID, ref List<string> lsError, ref List<string> lsSuccess)
        {
            if (testMarkList == null)
            {
                lsError.Add("no data found in request");
                return 0;
            }
            bool dbFlag = false;
            var test = _uofRepository.TestRepository.GetTestByTestId(testID, ref dbFlag);
            if (test == null)
            {
                lsError.Add("test does not exist");
                return 0;
            }

            var teacherClassSubject = _uofRepository.TeacherClassSubjectRepository.GetTeacherClassSubjectByID(test.TeacherClassSubjectID  , ref dbFlag);
            if (teacherClassSubject == null)
            {
                lsError.Add("teacher class subject does not exist");
                return 0;
            }

            var enrollment = _uofRepository.StudentTermRegisterRepository.GetListByClassIDAndTermID(teacherClassSubject.ClassID, teacherClassSubject.TermID, ref dbFlag);

            var newListForClass = new List<TestMark>();
            foreach (var item in testMarkList)
            {
                var isStudentExist = enrollment.Where(c => c.RegNumber?.ToLower() == item?.RegNumber?.ToLower()).FirstOrDefault();
                if (isStudentExist == null)
                {
                    lsError.Add($"{item.RegNumber} is not registered in class ");
                    continue;
                }
                newListForClass.Add(new TestMark()
                {
                    StudentTermRegisterID = isStudentExist.StudentTermRegisterID,
                    TestID = testID ,
                    Mark = item.Mark
                }
                );
            }
            return Save(newListForClass, testID, ref lsError, ref lsSuccess);
        }

        public int Save(List<TestMark> testMarkList, Guid testID, ref List<string> lsError, ref List<string> lsSuccess)
        {
            /*
             * 
             1: get Test 
             2: validate teacher
             3: check if student mark < out of
             4: check if student belongs to class 
             5: save
             */
            StringBuilder sbError = new StringBuilder("");
            var isClosed = IsTestSubmissionClosed(ref sbError);

            if (isClosed)
            {
                lsError.Add(sbError.ToString());
                return 0;
            }
            bool dbFlag = false;
            List<TestMark> filteredTest = new List<TestMark>();
            var test = _uofRepository.TestRepository.GetTestByTestId(testID, ref dbFlag);
            if (test == null)
            {
                lsError.Add("Test does not exist");
                return 0;
            }

            var teacherClassSubject = _uofRepository.TeacherClassSubjectRepository.GetTeacherClassSubjectByID(test.TeacherClassSubjectID, ref dbFlag);
            if (teacherClassSubject == null)
            {
                lsError.Add("teacher class subject does not exist");
                return 0;
            }

            var studentInClassByEnrollment = _uofRepository.StudentTermRegisterRepository.GetListByClassIDAndTermID(teacherClassSubject.ClassID, _user.TermID, ref dbFlag);
            if (studentInClassByEnrollment == null)
            {
                lsError.Add("No student enrolled in that class ");
                return 0;
            }
            foreach (var tes in testMarkList)
            {
                var isStudentExistInClass = studentInClassByEnrollment.Where(c => c.StudentTermRegisterID == tes.StudentTermRegisterID).FirstOrDefault();
                if (isStudentExistInClass == null)
                {
                    lsError.Add("student not enrolled in  class");
                    continue;
                }
                else
                {
                    tes.StudentTermRegisterID = (Guid)isStudentExistInClass.StudentTermRegisterID;
                }

                if (tes.Mark < 0)
                {
                    lsError.Add($"{isStudentExistInClass?.RegNumber } - Mark should be greater than zero ");
                    continue;
                }
                else if (tes.Mark > test.OutOf)
                {
                    lsError.Add($"{isStudentExistInClass?.RegNumber } - Mark should be less than Out of ");
                    continue;
                }

                filteredTest.Add(new TestMark()
                {
                    StudentTermRegisterID = tes.StudentTermRegisterID,
                    Mark = tes.Mark,
                    TestID = testID ,
                    Percentage = Convert.ToByte(( (decimal)tes.Mark / (decimal)test.OutOf) * (decimal)100)
                });
            }

            if (filteredTest == null) return 0;
            var numberAffected = _uofRepository.TestMarkRepository.Save(filteredTest, _user.Username, ref dbFlag);
            lsSuccess.Add($"{numberAffected} record(s) saved succcessfully");
            _uofRepository.LogRepository.Save(new Log() { TeacherID = _user.TeacherID, EntityType = "TestMark", ActionDetails = $"{numberAffected} records saved ", ActionType = "Saved" }, ref dbFlag);
            return numberAffected;
        }


        public List<TestMarkDto> ExcelSession(List<TestMarkDto> testMarkList, Guid testID, ref List<string> lsError)
        {
            /*
             * 
             1: get Test 
             2: validate teacher
             3: check if student mark < out of
             4: check if student belongs to class 
             5: save
             */
            StringBuilder sbError = new StringBuilder("");
            var isClosed = IsTestSubmissionClosed(ref sbError);

            if (isClosed)
            {
                lsError.Add(sbError.ToString());
                return null;
            }
            bool dbFlag = false;
            List<TestMarkDto> filteredTest = new List<TestMarkDto>();
            var test = _uofRepository.TestRepository.GetTestByTestId(testID, ref dbFlag);
            if (test == null)
            {
                lsError.Add("Test does not exist");
                return null;
            }

            var studentInClassByEnrollment = _uofRepository.StudentTermRegisterRepository.GetListByClassIDAndTermID(test.TeacherClassSubject.ClassID, _user.TermID, ref dbFlag);
            if (studentInClassByEnrollment == null)
            {
                lsError.Add("No student enrolled in that class ");
                return null;
            }
            foreach (var tes in testMarkList)
            {
                var isStudentExistInClass = studentInClassByEnrollment.Where(c => c.RegNumber == tes.RegNumber.Trim().ToLower()).FirstOrDefault();
                if (isStudentExistInClass == null)
                {
                    lsError.Add("student not enrolled in  class");
                    continue;
                }
                else
                {
                    tes.StudentTermRegisterID = (Guid)isStudentExistInClass.StudentTermRegisterID;
                }

                if (tes.Mark < 0)
                {
                    lsError.Add("Mark should be greater than zero ");
                    continue;
                }
                else if (tes.Mark > test.OutOf)
                {
                    lsError.Add("Mark should be less than Out of ");
                    continue;
                }

                filteredTest.Add(new TestMarkDto()
                {
                    StudentTermRegisterID = tes.StudentTermRegisterID,
                    RegNumber = tes.RegNumber ,
                    Mark = tes.Mark,
                    TestID = testID
                });
            }
            
            return filteredTest;
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

        public bool SoftDeleteMark(List<Guid> testList, Guid testID, ref int numberOfRecordsDeleted, ref StringBuilder sbError)
        {
            numberOfRecordsDeleted = 0;
            if (testList == null || testList.Count() <= 0)
            {
                sbError.Append("Not data found");
                return false;
            }

            bool dbFlag = false;

            var isClosed = IsTestSubmissionClosed(ref sbError);

            if (isClosed)
            {
                return false;
            }

            var test = _uofRepository.TestRepository.GetTestByTestId(testID, ref dbFlag);
            if(test == null)
            {
                sbError.Append("Test does not exist");
                return false;
            }
            // check submission deadline has been closed
            var isResponsibleForSubject = _uofRepository.TeacherClassSubjectRepository.GetTeacherClassSubjectByID(test.TeacherClassSubjectID, ref dbFlag);

            var studentEnrollment = _uofRepository.TestMarkRepository.GetListTestMarksByTestId((Guid)test.TestID, ref dbFlag);
            if (isResponsibleForSubject == null)
            {
                sbError.Append("Record for teacher class subject does not exist or has been deleted");
                return false;
            }
            else if (studentEnrollment == null)
            {
                sbError.Append("Student mark does not exist or it has been deleted , check deleted records");
                return false;
            }
            else
            {
                if (isResponsibleForSubject.TeacherID != _user.TeacherID)
                {
                    sbError.Append("You dont teacher that student");
                    return false;
                }
            }

            foreach (var testMarkEnrollemtID in testList)
            {
                if (studentEnrollment.Where(c => c.StudentTermRegisterID == testMarkEnrollemtID).FirstOrDefault() != null)
                {
                    var isDeleted = false;
                    isDeleted = _uofRepository.TestMarkRepository.Delete(testMarkEnrollemtID, testID, _user.Username,    ref dbFlag);
                    if (isDeleted)
                    {
                        numberOfRecordsDeleted++;
                    }
                }
            }
            _uofRepository.LogRepository.Save(new Log() { TeacherID = _user.TeacherID, EntityType = "TestMark", ActionDetails = $"{numberOfRecordsDeleted} records deleted ", ActionType = "Deleted" }, ref dbFlag);
            return true;
        }
    }
}
 