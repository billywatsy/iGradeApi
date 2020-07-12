using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class StudentService
    {
        private UowRepository _uofRepository;
        private Domain.Dto.LoggedUser _user;
        public StudentService(Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user;
        }

        public Student SaveSingle(Student student , ref StringBuilder sbError)
        {
            StringBuilder sbDbError = new StringBuilder("");
            if (!string.IsNullOrEmpty(student?.StudentID.ToString()))
            {
                Guid id = (Guid)student.StudentID;
                var studentDb = GetStudentById(id, ref sbDbError);

                if (studentDb == null)
                {
                    sbError.Append("Student does not exist");
                    return null;
                }
                else 
                {
                    if (_user.SchoolID != studentDb.SchoolID)
                    {
                        sbError.Append("Student does not belong to school");
                        return student;
                    }
                }
                student.SchoolID = studentDb.SchoolID;
                student.IDnational = studentDb.IDnational;
                student.StudentDateCreated = studentDb.StudentDateCreated;
                
                
            }
            else 
            {
                student.SchoolID = _user.SchoolID;
                student.StudentDateCreated = DateTime.Today;
            }
            

            if (student.DOB > DateTime.Today)
            {
                sbError.Append("DOB should be greater than today .");
            }

            bool dbErrorFlag = false;

            student = _uofRepository.StudentRepository.Save(student, _user.Username, ref dbErrorFlag);

            if (dbErrorFlag)
            {
                sbError.Append("Error saving student");
            }

            return student;
        }
        public bool IsStudentNumberPerSchoolLimitReached(ref StringBuilder sbError)
        {
            var dbSchoolFlag = false;
            var school = _uofRepository.SchoolRepository.GetSchoolBySchoolID(_user.SchoolID, ref dbSchoolFlag);

            if (dbSchoolFlag == true || school == null)
            {
                sbError.Append("Error failed getting school details when saving student");
                return false;
            }

            var numberEnrolled = _uofRepository.StudentRepository.GetNumberOfStudentsCreatedInSchoolByYear(_user.SchoolID , DateTime.Today , ref dbSchoolFlag);

            if (dbSchoolFlag == true)
            {
                sbError.Append("Error failed getting school number enrolled details when saving student");
                return false;
            }

            if (numberEnrolled > iGrade.Core.TeacherUserService.Common.PolicyCommon.OveralStudent)
            {
                sbError.Append("Error school has reached maximum of allowed students . Upgrade or to add more students");
                return false;
            }

            if(sbError.Length == 0)
            {
                return true;
            }
            {
                return false;
            }
        }

        public int SaveBulk(List<Student> students , List<string> ltErrors )
        {
            int recordsSaved = 0;
            StringBuilder sbError = new StringBuilder("");
            var isMaxReached = IsStudentNumberPerSchoolLimitReached(ref sbError);

            if (!isMaxReached)
            {
                return 0;
            }
            foreach (var student in students)
            {
               var saveNew = SaveSingle( student, ref sbError);

                if (saveNew == null)
                {
                    ltErrors.Add($"Failed saving student reg Number : {student.RegNumber} "+ sbError.ToString());
                }
                else
                {
                    recordsSaved++;
                }
            }
            return recordsSaved;
        }
        public Student GetStudentById(Guid studentID, ref StringBuilder dbError)
        {
            bool dbErrorFlag = false;
            var student = _uofRepository.StudentRepository.GetStudentById(studentID, ref dbErrorFlag);
            if (dbErrorFlag) dbError.Append("Error getting student");
            return student;
        }

        public List<Student> GetListSearchStudentByRegNumber(string regNumber, ref StringBuilder dbError)
        {
            bool dbErrorFlag = false;
            var student = _uofRepository.StudentRepository.GetListSearchStudentByRegNumberAndSchoolID(regNumber, _user.SchoolID, ref dbErrorFlag);
            if (dbErrorFlag) dbError.Append("Error getting student");
            return student;
        }

        public PagedList<Student> GetPagedListStudentSearch( int PageSize, int PageNo , string searchValue, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            PagedList<Student> list = _uofRepository.StudentRepository.GetPagedStudentSearch(_user.SchoolID, PageSize, PageNo, searchValue, ref dbFlag);
            return list;        
        }
        public List<Student> GetListStudentInRegNumberOrNationalID(List<string> ids, string schoolID, bool isRegNumber, ref StringBuilder dbError)
        {
            bool dbErrorFlag = false;
            var student = _uofRepository.StudentRepository.GetIsExistListStudentInRegNumberOrNationalID(ids , _user.SchoolID, isRegNumber , ref dbErrorFlag);
            if (dbErrorFlag) dbError.Append("Error getting student");
            return student;
        }

        
        public List<Student> IsExistStudent(List<Student> student , ref List<string> ltErrors)
        {
            bool dbFlag = false;
            if (student == null) return null;
            List<string> regNumbers = new List<string>();
            List<string> idNumbers = new List<string>();
            foreach (var stu in student)
            {
                regNumbers.Add(stu.RegNumber);
                idNumbers.Add(stu.IDnational);
            }

            var nationalIdList = _uofRepository.StudentRepository.GetIsExistListStudentInRegNumberOrNationalID(regNumbers, _user.SchoolID , true, ref dbFlag);
            var regNumberList = _uofRepository.StudentRepository.GetIsExistListStudentInRegNumberOrNationalID(idNumbers, _user.SchoolID  , false, ref dbFlag);

            if (regNumberList != null)
            {
                try
                {
                    foreach (var reg in regNumberList)
                    {
                        var removeNat = student.Where(c => c.RegNumber.ToLower() == reg.RegNumber.ToLower()).FirstOrDefault();
                        if (removeNat != null)
                        {
                            ltErrors.Add("Reg Number already exist");
                            student.Remove(removeNat);
                        }
                    }
                }
                catch
                {

                }
            }


            if (nationalIdList != null)
            {
                try
                {
                    foreach (var nat in nationalIdList)
                    {
                        var removeNat = student.Where(c => c.IDnational.ToLower() == nat.IDnational.ToLower()).FirstOrDefault();
                        if (removeNat != null)
                        {
                            ltErrors.Add("Id Number already exist");
                            student.Remove(removeNat);
                        }
                    }
                }
                catch 
                {

                }
                
            }
            return student;
        }

        public bool Delete(Guid studentId , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var student = _uofRepository.StudentRepository.GetStudentById(studentId, ref dbFlag);

            if(student == null)
            {
                sbError.Append("Student Does not Exist");
                return false;
            }

            if (student.SchoolID != _user.SchoolID)
            {
                sbError.Append("Error student does not exist for school");
                return false;
            }
            var enrollmnt = _uofRepository.StudentTermRegisterRepository.GetListAllByStudentID((Guid)student.StudentID , ref dbFlag);

            if(enrollmnt != null || enrollmnt?.Count() >= 1)
            {
                sbError.Append("Error student has some active records and can not be deleted for school");
                return false;
            }
           return  _uofRepository.StudentRepository.Delete((Guid)student.StudentID, _user.Username, ref dbFlag);
        }
    }
}
