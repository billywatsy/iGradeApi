using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iGrade.Domain.Dto;
using iGrade.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGrade.Api.Controllers.ParentApi
{
    [Route("api/p/[controller]")]
    [ApiController]
    public class StudentTermDataController :  BaseParent
    { 
        private UowRepository _repository;
        private ParentSessionDto _user;
        private StringBuilder _sbError;
        private Core.ParentService.StudentService _studentService;
        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository(); 
            _sbError = new StringBuilder("");
            _studentService = new Core.ParentService.StudentService(_user.Email, _user.SchoolCode, _sbError, _repository);
        }

        [HttpGet("mystudents")]
        public ActionResult<object> TermData()
        {
            try
            {
                Init();

                var listOfStudents = _studentService.GetMyStudents();
                var school = _studentService.GetSchoolInformation();
                return new
                {
                    student_list = listOfStudents ,
                    school_data = school
                };
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("getStudentTermRegistrations")]
        public ActionResult<object> GetStudentTermRegistrations(string regNumber)
        {
            try
            {
                Init();

                var listOfStudents = _studentService.GetTermStudentRegistered(regNumber) ?? new List<StudentTermRegisterDto>();

                var uniqueTestSubjects = _studentService.GetUniqueSubjectTestsWritten(regNumber) ?? new List<Domain.Subject>();

                var student = _studentService.GetStudent(regNumber) ?? new Domain.Student();

                var files = _studentService.GetStudentFiles(regNumber) ?? new List<TeacherClassSubjectFileDto>();
                return new 
                {
                    term_registered = listOfStudents , 
                    test_subjects = uniqueTestSubjects ,
                    student = student ,
                    files = files
                };
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("getStudentTermData")]
        public ActionResult<object> GetStudentTermData(string regNumber, int term, int year)
        {
            try
            {
                Init();

                var exams = _studentService.GetExamsByYearAndTerm(regNumber, year, term) ?? new List<ExamDto>();
                var goodReviws = _studentService.GetReviews(regNumber, year, term) ?? new List<StudentTermReviewDto>();

                return new
                {
                    exams = exams, 
                    term_reviews = goodReviws  
                };
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpGet("getStudentTestSubjectData")]
        public ActionResult<object> GetStudentTestSubjectData(string regNumber, string subjectCode)
        {
            try
            {
                Init();

                var marks = _studentService.GetSubjectLast20Tests(regNumber, subjectCode) ?? new List<TestMarkDto>();


                return marks;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }
         
    }
}
