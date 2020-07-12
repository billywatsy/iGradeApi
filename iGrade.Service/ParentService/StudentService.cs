using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.ParentService
{
    public class StudentService
    {
        private UowRepository _uofRepository;
        private string _parentEmail; 
        private string _schoolCode;
        StringBuilder _sbError;
        public StudentService(string __parentEmail ,string __schoolCode , StringBuilder __sbError, UowRepository uofRepository )
        {
            _parentEmail = __parentEmail;
            _sbError = __sbError;
            _schoolCode = __schoolCode;
            _uofRepository = uofRepository;
        }
        

        public Student GetStudent(string regNumber)
        {
            bool dbFlag = false;
            var student = _uofRepository.StudentRepository.GetStudentByRegNumberAndSchoolCode(regNumber, _schoolCode, ref dbFlag);
            return student;
        }

        public Domain.ParentDtos.SchoolDataDto GetSchoolInformation()
        {
            bool dbFlag = false;
            var student = _uofRepository.SchoolRepository.GetSchoolByCode(_schoolCode, ref dbFlag);

            var schoold = _uofRepository.SchoolRepository.GetSchoolBySchoolID((Guid)student.SchoolID, ref dbFlag) ?? new School();
            var moreinfo = _uofRepository.SchoolInformationRepository.GetById((Guid)student.SchoolID, ref dbFlag) ?? new SchoolInformation();

            var setting = _uofRepository.SettingRepository.GetSettingBySchoolID(schoold.SchoolID, ref dbFlag) ?? new Setting();

            var term = _uofRepository.TermRepository.GetByID(setting.TermID, ref dbFlag) ?? new Term();
            return new Domain.ParentDtos.SchoolDataDto
            {
                School = schoold , 
                SchoolInformation = moreinfo , 
                Term = term
            };
        }

        public List<Domain.Student> GetMyStudents()
        {
            bool dbFlag = false;

            var school = _uofRepository.SchoolRepository.GetSchoolByCode(_schoolCode, ref dbFlag);

            if (school == null)
            {
                _sbError.Append("School does not exist");
                return new List<Domain.Student>();
            }
            
            var students = _uofRepository.StudentRepository.GetListOfStudentsLinkedToEmailAndSchoolID(_parentEmail, (Guid)school.SchoolID, ref dbFlag) ?? new List<Student>();

            return students;
        }

        public List<Domain.Dto.StudentTermRegisterDto> GetTermStudentRegistered(string regNumber)
        {
            bool dbFlag = false;

            var student = GetStudent(regNumber);
            if (student == null)
            {
                return new List<Domain.Dto.StudentTermRegisterDto>();
            }

            var terms = _uofRepository.StudentTermRegisterRepository.GetListAllByStudentID((Guid)student.StudentID, ref dbFlag) ?? new List<Domain.Dto.StudentTermRegisterDto>();
            return terms;
        }


        public List<Domain.Subject> GetUniqueSubjectTestsWritten(string regNumber)
        {
            bool dbFlag = false;

            var student = GetStudent(regNumber);
            if (student == null)
            {
                return new List<Domain.Subject>();
            }

            var subjects = _uofRepository.TestMarkRepository.GetUniqueSubjectsTestsWrittenByStudent((Guid)student.StudentID, ref dbFlag) ?? new List<Subject>();
            return subjects;
        }

        public List<Domain.Dto.ExamDto> GetExamsByYearAndTerm(string regNumber , int year , int term )
        {
            bool dbFlag = false;

            var student = GetStudent(regNumber);
            if (student == null)
            {
                return new List<Domain.Dto.ExamDto>();
            }
            var school = _uofRepository.SchoolRepository.GetSchoolByCode(_schoolCode, ref dbFlag);

            if(school == null)
            {
                _sbError.Append("School does not exist");
                return new List<Domain.Dto.ExamDto>();
            }

            var termObj = _uofRepository.TermRepository.GetByTermNumberAndTermYearAndSchoolID((Guid)school.SchoolID ,term, year, ref dbFlag);

            if (school == null)
            {
                _sbError.Append("term does not exist");
                return new List<Domain.Dto.ExamDto>();
            }
            
            var studentTermRegister = _uofRepository.StudentTermRegisterRepository.GetByStudentIDAndTermId((Guid)student.StudentID, (Guid)termObj.TermID, ref dbFlag);
            if (studentTermRegister == null)
            {
                _sbError.Append($"student was not register for term:{term} and year:{year}");
                return new List<Domain.Dto.ExamDto>();
            }

            if (!studentTermRegister.CanParentViewExamData)
            {
                _sbError.Append($"student data for term:{term} and year:{year} is locked school admin , please contact school to view ");
                return new List<Domain.Dto.ExamDto>();
            }
            
            var exams = _uofRepository.ExamRepository.GetListByStudentIDAndTermID((Guid)student.StudentID, (Guid)termObj.TermID, ref dbFlag) ?? new List<Domain.Dto.ExamDto>();

            return exams;
        }


        public List<Domain.Dto.TeacherClassSubjectFileDto> GetRegNumber(string regNumber)
        {
            bool dbFlag = false;

            var student = GetStudent(regNumber);
            if (student == null)
            {
                return new List<Domain.Dto.TeacherClassSubjectFileDto>();
            }
            var school = _uofRepository.SchoolRepository.GetSchoolByCode(_schoolCode, ref dbFlag);

            if (school == null)
            {
                _sbError.Append("School does not exist");
                return new List<Domain.Dto.TeacherClassSubjectFileDto>();
            }

         //   var teacherClassSubject = _uofRepository.Te

          //  var termObj = _uofRepository.TermRepository.((Guid)school.SchoolID, school, year, ref dbFlag);

            if (school == null)
            {
                _sbError.Append("term does not exist");
                return new List<Domain.Dto.TeacherClassSubjectFileDto>();
            }

            //var studentTermRegister = _uofRepository.StudentTermRegisterRepository.GetByStudentIDAndTermId((Guid)student.StudentID, (Guid)termObj.TermID, ref dbFlag);
            
            return  null;
        }



        public List<Domain.Dto.TestMarkDto> GetSubjectLast20Tests(string regNumber, string subjectCode)
        {
            bool dbFlag = false;

            var student = GetStudent(regNumber);
            if (student == null)
            {
                return new List<Domain.Dto.TestMarkDto>();
            }
            var school = _uofRepository.SchoolRepository.GetSchoolByCode(_schoolCode, ref dbFlag);

            if (school == null)
            {
                _sbError.Append("School does not exist");
                return new List<Domain.Dto.TestMarkDto>();
            }

            var subject = _uofRepository.SubjectRepository.GetSubjectByCode(subjectCode,(Guid)school.SchoolID , ref dbFlag);

            if (subject == null)
            {
                _sbError.Append("subject does not exist");
                return new List<Domain.Dto.TestMarkDto>();
            }

            var marks = _uofRepository.TestMarkRepository.GetListTestMarksStudentIDAndSubjectIDLatestByN(20 , (Guid)student.StudentID,(Guid)subject.SubjectID, ref dbFlag);

            return marks;
        }


        public List<Domain.Dto.StudentTermReviewDto> GetReviews(string regNumber, int year, int term)
        {
            bool dbFlag = false;

            var student = GetStudent(regNumber);
            if (student == null)
            {
                return new List<Domain.Dto.StudentTermReviewDto>();
            }
            var school = _uofRepository.SchoolRepository.GetSchoolByCode(_schoolCode, ref dbFlag);

            if (school == null)
            {
                _sbError.Append("School does not exist");
                return new List<Domain.Dto.StudentTermReviewDto>();
            }

            var termObj = _uofRepository.TermRepository.GetByTermNumberAndTermYearAndSchoolID((Guid)school.SchoolID, term, year, ref dbFlag);

            if (school == null)
            {
                _sbError.Append("term does not exist");
                return new List<Domain.Dto.StudentTermReviewDto>();
            }

            var studentTermRegister = _uofRepository.StudentTermRegisterRepository.GetByStudentIDAndTermId((Guid)student.StudentID, (Guid)termObj.TermID, ref dbFlag);
            if (studentTermRegister == null)
            {
                _sbError.Append($"student was not register for term:{term} and year:{year}");
                return new List<Domain.Dto.StudentTermReviewDto>();
            }

            var reviews = _uofRepository.StudentTermReviewRepository.GetListByStudentTermRegisterID((Guid)studentTermRegister.StudentTermRegisterID, ref dbFlag) ?? new List<Domain.Dto.StudentTermReviewDto>();

            reviews = reviews.Where(c => c.IsReviewGood).ToList(); 
            return reviews ?? new List<Domain.Dto.StudentTermReviewDto>(); ;
        }



        public List<Domain.Dto.TeacherClassSubjectFileDto> GetStudentFiles(string regNumber)
        {
            bool dbFlag = false;
            var student = GetStudent(regNumber);
            if (student == null)
            {
                return new List<Domain.Dto.TeacherClassSubjectFileDto>();
            }
            var school = _uofRepository.SchoolRepository.GetSchoolByCode(_schoolCode, ref dbFlag);

            if (school == null)
            {
                _sbError.Append("School does not exist");
                return new List<Domain.Dto.TeacherClassSubjectFileDto>();
            }

            var setting = _uofRepository.SettingRepository.GetSettingBySchoolID((Guid)school.SchoolID, ref dbFlag);

            if (setting == null)
            {
                _sbError.Append("school settings does not exist");
                return new List<Domain.Dto.TeacherClassSubjectFileDto>();
            }

            var term = _uofRepository.TermRepository.GetByID(setting.TermID, ref dbFlag);
            if (term == null)
            {
                _sbError.Append("school term does not exist");
                return new List<Domain.Dto.TeacherClassSubjectFileDto>();
            }
            var termreg = _uofRepository.StudentTermRegisterRepository.GetByStudentIDAndTermId((Guid)student.StudentID, (Guid)term.TermID, ref dbFlag);
            if (termreg == null)
            {
                _sbError.Append("student not registered in current term");
                return new List<Domain.Dto.TeacherClassSubjectFileDto>();
            }

            var fileTypes = _uofRepository.TeacherClassSubjectFileTypeRepository.GetListBySchoolId((Guid)school.SchoolID, ref dbFlag);
            if (fileTypes == null)
            {
                _sbError.Append("no file types in current term");
                return new List<Domain.Dto.TeacherClassSubjectFileDto>();
            }

            var subjectFile = _uofRepository.TeacherClassSubjectFileRepository.GetListByTermIdAndClassId(termreg.ClassID, termreg.TermID, ref dbFlag);

            if (subjectFile == null)
            {
                _sbError.Append("no files in current term");
                return new List<Domain.Dto.TeacherClassSubjectFileDto>();
            }


            List<Domain.Dto.TeacherClassSubjectFileDto> parentFile = new List<Domain.Dto.TeacherClassSubjectFileDto>();

            foreach (var file in subjectFile)
            {
                var fileType = fileTypes.FirstOrDefault(c => c.TeacherClassSubjectFileTypeId == file.TeacherClassSubjectFileTypeId);

                if(fileType == null)
                {
                    continue;
                }

                if (fileType.CanParentAccessFile)
                {
                    parentFile.Add(file);
                }
            }
            return parentFile;
        }



    }
}




