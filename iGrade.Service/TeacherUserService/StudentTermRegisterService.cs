using iGrade.Domain;
using iGrade.Domain.Dto;
using iGrade.Domain.Form; 
using iGrade.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class StudentTermRegisterService
    {
        private UowRepository _uofRepository;
        private LoggedUser _user;

        public StudentTermRegisterService(LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user;
        }

          
        public List<StudentTermRegisterDto> GetClassListByClassIDAndSchoolIDAndTermID(Guid classID, Guid termID , ref StringBuilder dbFlag)
        {
            bool dbError = false;
            var list = _uofRepository.StudentTermRegisterRepository.GetClassListByClassIDAndSchoolIDAndTermID(classID, _user.SchoolID, termID, ref dbError);
            return list;
        }

        public List<StudentTermRegisterDto> GetListBySchoolIDAndCurrentTermID(ref StringBuilder dbFlag)
        {
            bool dbError = false;
            var list = _uofRepository.StudentTermRegisterRepository.GetListBySchoolIDAndTermID(_user.SchoolID, _user.TermID, ref dbError);
            if (list == null)
            {
                list = new List<StudentTermRegisterDto>();
            }
            return list;
        }


        public StudentTermRegister GetByID(Guid studentTermRegisterID, ref StringBuilder dbFlag)
        {
            bool dbError = false;
            var enrol = _uofRepository.StudentTermRegisterRepository.GetByID(studentTermRegisterID, ref dbError);
            return enrol;
        }

        public StudentTermRegister GetByRegNumberAndTermID(string regNumber, Guid termID , ref StringBuilder dbFlag)
        {
            bool dbError = false;

            var student = _uofRepository.StudentRepository.GetStudentByRegNumber(regNumber, _user.SchoolID, ref dbError);

            if(student == null)
            {
                return null;
            }

            
            var enrol = _uofRepository.StudentTermRegisterRepository.GetByStudentIDAndTermId((Guid)student.StudentID , _user.SchoolID, _user.TermID, ref dbError);
            return enrol;
        }
         public string BulkSave(List<StudentTermRegisterFORM> enrollmentFORMs, ref List<string> sbError, ref List<string> sbSuccess)
        {
            if (enrollmentFORMs == null)
            {
                return null;
            }
            var classID = enrollmentFORMs.Select(c => c.ClassID).FirstOrDefault();
            var termID = enrollmentFORMs.Select(c => c.TermID).FirstOrDefault();
            bool dbFlag = false;
            var schoolClass = _uofRepository.ClassRepository.GetClassByID(classID, ref dbFlag);
            var studentsListEnrolledInClass = _uofRepository.StudentTermRegisterRepository.GetListByClassIDAndTermID(classID, _user.TermID, ref dbFlag);

            foreach (var enrollmentForm in enrollmentFORMs)
            {

                var student = _uofRepository.StudentRepository.GetStudentByRegNumber(enrollmentForm.RegNumber, _user.SchoolID, ref dbFlag);

                if (student == null)
                {
                    sbError.Add("Student Reg Number does not exist");
                    continue;
                }

                if (student.SchoolID != _user.SchoolID)
                {
                    sbError.Add("Student does not  exist");
                    continue;
                }
                if (schoolClass == null)
                {
                    sbError.Add("Class does not exist for school");
                    continue;
                }

                var enrollment = new StudentTermRegister()
                {
                    StudentTermRegisterID = null,
                    ClassID = enrollmentForm.ClassID,
                    StudentID = (Guid)student.StudentID,
                    TermID = enrollmentForm.TermID,
                    IsAllowedSent = enrollmentForm.IsAllowedSent,
                    IsEmailSent = enrollmentForm.IsEmailSent,
                    IsPhoneSent = enrollmentForm.IsPhoneSent
                };

                enrollment = _uofRepository.StudentTermRegisterRepository.Insert(enrollment, _user.Username , ref dbFlag);
                if (dbFlag)
                {
                    sbError.Add("Error getting saving enrollement");
                    continue;
                }
                if (enrollment != null)
                {
                    sbSuccess.Add(enrollmentForm.RegNumber);
                }
                else
                {
                    sbError.Add(enrollmentForm.RegNumber + " Failed to save please try again");
                }
            }
            return null;
        }

        public int ImportFromTermToAnotherTerm(Guid fromTermID, Guid toTermID, ref StringBuilder sbError)
        {
            if (fromTermID == toTermID)
            {
                sbError.Append("Term should be different");
                return 0;
            }
            var dbFlag = false;
            var schoolTermList = _uofRepository.TermRepository.GetListTermBySchoolID(_user.SchoolID, ref dbFlag);

            if (schoolTermList == null)
            {
                if ((schoolTermList.Where(c => c.TermID == fromTermID).FirstOrDefault()) == null)
                {
                    sbError.Append("School From Term Does Not Exist");
                    return 0;
                }
                if ((schoolTermList.Where(c => c.TermID == toTermID).FirstOrDefault()) == null)
                {
                    sbError.Append("School To Term Does Not Exist");
                    return 0;
                }
            }
            var listFromTerm = _uofRepository.StudentTermRegisterRepository.GetListBySchoolIDAndTermID(_user.SchoolID, fromTermID, ref dbFlag);

            if (listFromTerm == null)
            {
                return 0;
            }

            List<StudentTermRegister> listTransfered = new List<StudentTermRegister>();
            foreach (var enrollmentForm in listFromTerm)
            {
                listTransfered.Add
                (new StudentTermRegister()
                {
                    StudentTermRegisterID = null,
                    ClassID = enrollmentForm.ClassID,
                    StudentID = (Guid)enrollmentForm.StudentID,
                    TermID = toTermID,
                    IsAllowedSent = true,
                    IsEmailSent = true,
                    IsPhoneSent = true
                }
                );
            }
            var numberInserted = _uofRepository.StudentTermRegisterRepository.BulkInsert(listTransfered, _user.Username , ref dbFlag);
            if (dbFlag)
            {
                sbError.Append("Error Saving list");
            }
            return numberInserted;
        }
        /// <summary>
        /// A: Class Code , 
        /// B: Reg Number
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public object SaveList(List<ExcelPostListDto> list )
        {
            var numberOfSuccessSave = 0;
            List<string> ltErrors = new List<string>();

            if(list == null || ltErrors?.Count() <= 0)
            {
                ltErrors.Add("No data found in list");
                return new
                {
                    success = numberOfSuccessSave,
                    error = ltErrors
                };
            }
            bool dbFlag = false;
            var classList = _uofRepository.ClassRepository.GetListClassesBySchoolID(_user.SchoolID, ref dbFlag);
            if (classList == null || classList?.Count() <= 0)
            {
                ltErrors.Add("Failed getting classes");
                return new
                {
                    success = numberOfSuccessSave,
                    error = ltErrors
                };
            }
            int row = 1;
            foreach (var student in list)
            {
                row++;
                if (string.IsNullOrEmpty(student.A) || string.IsNullOrEmpty(student.B))
                {
                    ltErrors.Add("No data found in row "+ row);
                    continue;
                } 
                var studentExist = _uofRepository.StudentRepository.GetStudentByRegNumber(student.A,_user.SchoolID , ref dbFlag);

                if(studentExist == null)
                {
                    ltErrors.Add($"Student with reg number {student.A} does not exist for school "  );
                    continue;
                }
                else
                {
                    var isClassCodeExist = classList.FirstOrDefault(c => c.ClassCode == student.B);
                    if(isClassCodeExist == null)
                    {

                        ltErrors.Add($"Student with reg number {student.A} and class code {student.B} class code does not exist for school");
                        continue;
                    }
                    StringBuilder sbError = new StringBuilder("");
                   var isSaved =  SingleSave(new StudentTermRegisterFORM() {
                    RegNumber = studentExist.RegNumber ,
                       StudentTermRegisterID = null ,
                    ClassID = (Guid)isClassCodeExist.ClassID ,
                    TermID = _user.TermID ,
                    IsPhoneSent = true , 
                    IsEmailSent = true ,
                    IsAllowedSent = true 
                    } , ref sbError);

                    if(isSaved == null)
                    {
                        ltErrors.Add($"Error reg number {student.A} save " + sbError.ToString());
                    }
                    else
                    {
                        numberOfSuccessSave++;
                    }
                }
            }


            return new
            {
                success = numberOfSuccessSave,
                error = ltErrors
            };
        }

        public StudentTermRegister SingleSave(StudentTermRegisterFORM enroll, ref StringBuilder sbError)
        {
            Student student = null;
            StudentTermRegister studentDataEnrollment = null;
            bool dbFlag = false;

            if (!_user.isAdmin)
            {
                if (_user.ClassID != null)
                {
                    if (!_user.ClassID.Equals(enroll.ClassID))
                    {
                        sbError.Append("You are not class teacher of that class");
                        return null;
                    }
                }
                else
                {
                    sbError.Append("You are not allowed to register student for this class");
                    return null;
                }
            }
           
            

            var school = _uofRepository.SchoolRepository.GetSchoolBySchoolID(_user.SchoolID, ref dbFlag);

            
            student = _uofRepository.StudentRepository.GetStudentByRegNumber(enroll.RegNumber, _user.SchoolID, ref dbFlag);

            if (enroll.StudentTermRegisterID != null && enroll.StudentTermRegisterID != Guid.Empty)
            {
                var isExist = _uofRepository.StudentTermRegisterRepository.GetByID((Guid)enroll.StudentTermRegisterID, ref dbFlag);
                if (isExist == null)
                {
                    sbError.Append("Student not found by for term");
                    return null;
                }
                else
                {
                    studentDataEnrollment = isExist;
                }
            }

            
            if (student == null)
            {
                sbError.Append("Student not found by reg number for school");
                return null;
            }
             
            
            if (student == null)
            {
                sbError.Append("Student does not exist for school");
                return null;
            }
            if (dbFlag)
            {
                sbError.Append("Error getting enrollment details");
                return null;
            }
            if (student.SchoolID != _user.SchoolID)
            {
                sbError.Append("Student does not belong to school exist");
                return null;
            }
            if (student.SchoolID != _user.SchoolID)
            {
                sbError.Append("Student does not belong to school exist");
                return null;
            }
          


            
            if (enroll.StudentTermRegisterID != null && enroll.StudentTermRegisterID != Guid.Empty)
            {
                studentDataEnrollment.IsAllowedSent = enroll.IsAllowedSent;
                studentDataEnrollment.IsEmailSent = enroll.IsEmailSent;
                studentDataEnrollment.IsPhoneSent = enroll.IsPhoneSent;
                var e = _uofRepository.StudentTermRegisterRepository.Update(studentDataEnrollment, _user.Username, ref dbFlag);
                return e;
            }                 

            var schoolClass = _uofRepository.ClassRepository.GetClassByID(enroll.ClassID, ref dbFlag);
            if (schoolClass == null)
            {
                sbError.Append("Class does not exist for school");
                return null;
            }

            if (dbFlag)
            {
                sbError.Append("Error getting enrollment details");
                return null;
            }
            
            if (schoolClass.SchoolID != _user.SchoolID)
            {
                sbError.Append("Class does not exist for school or vice versa");
                return null;
            }

            StudentTermRegister enrollment  = new StudentTermRegister()
            {
                StudentTermRegisterID = null,
                ClassID = enroll.ClassID,
                StudentID = (Guid)student.StudentID,
                TermID = _user.TermID,
                IsAllowedSent = enroll.IsAllowedSent,
                IsEmailSent = enroll.IsEmailSent,
                IsPhoneSent = enroll.IsPhoneSent
            };


            enrollment = _uofRepository.StudentTermRegisterRepository.Insert(enrollment, _user.Username , ref dbFlag);
            if (dbFlag)
            {
                sbError.Append("Error getting saving enrollement");
                return null;
            }

            // check if chan
            return enrollment;
        }

        public List<StudentTermRegisterDto> GetListStudentEnrolledInClass(Guid classId, Guid termID, ref StringBuilder sbError)
        {
            bool dbFlag = false;

            var lis = _uofRepository.StudentTermRegisterRepository.GetListByClassIDAndTermID(classId, termID, ref dbFlag);
            return lis;
        }

        public List<StudentTermRegisterDto> GetListBySchoolIDAndTermID(Guid termID, ref StringBuilder sbError)
        {
            bool dbFlag = false;

            return _uofRepository.StudentTermRegisterRepository.GetListBySchoolIDAndTermID(_user.SchoolID, termID, ref dbFlag);
        }

         
        

        public List<StudentTermRegisterDto> GetStudentsByTestId(Guid testID, ref StringBuilder sbError)
        {
            bool dbFlag = false;

            var test = _uofRepository.TestRepository.GetTestByTestId(testID, ref dbFlag);

            if (test == null)
            {
                return null;
            }

            var teacherClassStudent = _uofRepository.TeacherClassSubjectRepository.GetTeacherClassSubjectByID(test.TeacherClassSubjectID, ref dbFlag);
            if (teacherClassStudent == null)
            {
                return null;
            }
            
            var enrollment = _uofRepository.StudentTermRegisterRepository.GetClassListByClassIDAndSchoolIDAndTermID(teacherClassStudent.ClassID , _user.SchoolID , teacherClassStudent.TermID, ref dbFlag);
            return enrollment;
        }

        public List<StudentTermRegisterDto> GetListByTeacherClassSubjectID(Guid teacherClassSubjectID, ref StringBuilder sbError)
        {
            bool dbFlag = false;

            var teacherClassStudent = _uofRepository.TeacherClassSubjectRepository.GetTeacherClassSubjectByID(teacherClassSubjectID, ref dbFlag);
            if (teacherClassStudent == null)
            {
                sbError.Append("teacher class subject does not exist");
                return null;
            }
            var enrollment = _uofRepository.StudentTermRegisterRepository.GetListByClassIDAndTermID(teacherClassStudent.ClassID , teacherClassStudent.TermID, ref dbFlag);
            return enrollment;
        }



        public bool Delete(Guid enrollmentId, ref StringBuilder sbError)
        {
            bool dbFlag = false;

            var student = _uofRepository.StudentTermRegisterRepository.GetByID(enrollmentId, ref dbFlag);

            if (student == null)
            {
                sbError.Append("student does not exist for school");
                return false;
            }

            /*
             check if studentt has test or exam data for key constaint 
             */

            var exams = _uofRepository.ExamRepository.GetListExamByStudentTermRegisterIDAndTermID(enrollmentId, _user.TermID, ref dbFlag);
            if (dbFlag) { sbError.Append("Error getting student check exams before delete"); }
            var tests = _uofRepository.TestMarkRepository.GetListTestMarksByStudentTermRegisterIDAndTermID(enrollmentId, _user.TermID, ref dbFlag);
            if (dbFlag) { sbError.Append("Error getting student check tests before delete"); }
            var absents = _uofRepository.AbsentFromSchoolRepository.GetListAbsentByStudentTermRegisterID(enrollmentId, ref dbFlag);
            if (dbFlag) { sbError.Append("Error getting student check absents before delete"); }
            var termreview = _uofRepository.StudentTermReviewRepository.GetListByStudentTermRegisterID(enrollmentId, ref dbFlag);
            if (dbFlag) { sbError.Append("Error getting student check term reviews before delete"); }

            if (exams != null && exams?.Count() >= 1)
            {
                sbError.Append($"{exams?.Count()} exam records dependant on the studet term enrollment . ");
            }
            if (tests != null && tests?.Count() >= 1)
            {
                sbError.Append($"{tests?.Count()} tests records dependant on the enrollment data . ");
            }
            if (absents != null && absents?.Count() >= 1)
            {
                sbError.Append($"{absents?.Count()} absents records dependandant on the enrollment data . ");
            }
            if (termreview != null && termreview?.Count() >= 1)
            {
                sbError.Append($"{termreview?.Count()} student term review records dependandant on the enrollment data . ");
            }

            if (!string.IsNullOrEmpty(sbError.ToString()))
            {
                return false;
            }


            var delete = _uofRepository.StudentTermRegisterRepository.DeleteByID(enrollmentId, _user.Username , ref dbFlag);

            return delete;
        }
    }

}
