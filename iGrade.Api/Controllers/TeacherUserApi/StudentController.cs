using iGrade.Core.TeacherUserService;
using iGrade.Domain;
using iGrade.Domain.Dto;
using iGrade.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avo;
using iGrade.Api.Controllers.TeacherUserApi.Model;

namespace iGrade.Api.Controllers
{
    [Route("api/s/[controller]")]
    [ApiController]
    public class StudentController : BaseUser
    {
        private StudentService _studentService;
        private LevelService _levelService;
        private ClassService _classService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository(); 
            _classService = new ClassService(_user, _repository);
            _studentService = new StudentService(_user, _repository);
            _levelService = new LevelService(_user, _repository);

            _sbError = new StringBuilder("");
        }

        [HttpGet]
        public ActionResult<object> Search(int pageSize = 10 , int pageNumber = 1  ,string q = null)
        {
            try
            {
                Init();
                return _studentService.GetPagedListStudentSearch(pageSize, pageNumber, q, ref _sbError);
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<object> Get(Guid? id)
        {
            try
            {
                Init();
                if (id != null && Guid.Empty != id)
                {
                    return _studentService.GetStudentById((Guid)id, ref _sbError);
                }
                return null;
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]StudentFormUpsert form)
        {
            try
            {
                Init(); 
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400; 
                    return "Failed getting student id" ; 
                }
                else
                {
                    var studentSaved = _studentService.SaveSingle(
                        new Student { 
                                    StudentID = form.StudentID ,
                                    SchoolID = _user.SchoolID ,
                                    StudentName = form.StudentName ,
                                    StudentMidName = form.StudentMidName ,
                                    StudentSurname = form.StudentSurname,
                                    RegNumber = form.RegNumber ,
                                    IDnational = form.IDnational ,
                                    IsMale = form.IsMale ,
                                    DOB = form.DOB ,
                                    Email = form.Email  ,
                                    ContactEmail = form.ContactEmail ,
                                    Phone =form.Phone
                        }
                        , ref sbError);
                    if (studentSaved == null)
                    {
                        Response.StatusCode = 400;
                        return "student save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return Ok(new { id = studentSaved.StudentID });
                    }
                }
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("savelist")]
        public ActionResult<object> SaveList([FromBody] List<ExcelPostListDto> list)
        {
            try
            {
                Init();
                if (list == null || list?.Count() <= 0)
                {
                    return new BadRequestObjectResult("no data found");
                }
                List<string> ltErrors = new List<string>();
                List<Student> students = new List<Student>();
                foreach (var item in list)
                {
                    var isError = false;
                    var isMale = false;

                    try
                    {
                        isMale = item.E.IsMale();
                    }
                    catch (Exception er)
                    {
                        ltErrors.Add($"Reg Number: for {item.A} has wrong gender " + er.Message);
                        isError = true;
                    }

                    var dob = DateTime.Now;
                    if(!DateTime.TryParse(item.H, out dob))
                    {
                        ltErrors.Add($"Reg Number: {item.A} has wrong dob ");
                        isError = true;
                    }

                    if (!isError)
                    {
                        students.Add(
                        new Student()
                        {
                            RegNumber = item.A,
                            StudentName = item.B,
                            StudentMidName = item.C,
                            StudentSurname = item.D,
                            IsMale = isMale,
                            Phone = item.F,
                            Email = item.G,
                            DOB = dob,
                            IDnational = item.I , 
                            SchoolID = _user.SchoolID
                        });
                    }
                }
                
                var number = _studentService.SaveBulk(students, ltErrors);
                
                return new {
                            success = number , 
                            error = ltErrors
                        };
                
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("Delete")]
        public ActionResult<object> Delete([FromBody] DeleteID deleteID)
        {
            try
            {
                Init();
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "Student does not exist" ;
                }
                else
                {
                    if (deleteID.id == null)
                    {
                        Response.StatusCode = 400;
                        return "Student does not exist" ;
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _studentService.Delete(deleteID.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return  "Student Delete failed or "+ sbError.ToString();
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"Student Deleted Successfully";
                    }
                }
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }
    }
}