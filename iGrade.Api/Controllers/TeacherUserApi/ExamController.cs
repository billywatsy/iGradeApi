using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iGrade.Core.TeacherUserService;
using iGrade.Domain;
using iGrade.Domain.Dto;
using iGrade.Domain.Form;
using iGrade.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGrade.Api.Controllers.TeacherUserApi
{
    [Route("api/s/[controller]")]
    [ApiController]
    public class ExamController : BaseUser
    { 
        private ExamService _examService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _examService = new ExamService(_user , _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet("{id}/teacherclasssubject")]
        public ActionResult<object> GetListExamByTeacherClassSubjectID(Guid id)
        {
            try
            {
                Init();
                return _examService.GetListExamByTeacherClassSubjectID(id ,false,ref _sbError);
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost]
        public ActionResult<object> Save([FromBody]Exam exam)
        {
            try
            {
                Init();
                
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "Failed getting exam id";
                }
                else
                {
                    var listSuccess = new List<string>();
                    var listError = new List<string>();
                    var list = new List<Exam>();
                    list.Add(exam);


                    var isSaved = _examService.Save(list, exam.TeacherClassSubjectID, ref listError, ref listSuccess);
                    if (isSaved <= 0)
                    {
                        Response.StatusCode = 400;
                        return "exam save failed " + string.Join(", " , listError);
                    }
                    else
                    {
                        return (string)"exam saved successfully";
                    }

                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("{teacherClassSubjectID}/savelist")]
        public ActionResult<object> Save(Guid teacherClassSubjectID , [FromBody]List<ExcelPostListDto> list)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "Failed getting exam id";
                }
                else
                {
                    var listSuccess = new List<string>();
                    var listError = new List<string>();  

                    if(list == null)
                    {
                        return "";
                    }
                    List<ExamSaveFORM> exam = new List<ExamSaveFORM>();

                    foreach (var item in list)
                    {
                        if (byte.TryParse(item.B, out byte mark))
                        {
                            exam.Add(new ExamSaveFORM()
                            {
                                RegNumber = item.A,
                                Mark = mark,
                                Comment = item.C
                            });
                        }
                        else
                        {
                            listError.Add($"Reg Number : {item.A} has incorrect mark format");
                        }
                    }

                    var numberSaved = _examService.SaveBulkByRegNumber(exam, teacherClassSubjectID, ref listError, ref listSuccess);
                    return new { success = numberSaved, error = listError };

                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        
        [HttpPost("Delete")]
        public ActionResult<object> DeleteExam([FromBody]Model.DeleteExam delExam)
        {
            Init();
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return "Exam does not exist"  ;
            }
            else
            {

                if (delExam.studentTermRegisterID == null)
                {
                    Response.StatusCode = 400;
                    return  "Exam does not exist" ;
                }
                StringBuilder sbError = new StringBuilder("");

                List<Guid> list = new List<Guid>();
                list.Add(delExam.studentTermRegisterID);
                int numberDeleted = 0;
                var isDeleted = _examService.Delete(list, delExam.teacherClassSubjectID, ref numberDeleted , ref sbError);

                if (!isDeleted)
                {
                    Response.StatusCode = 400;
                    return  "Exam Delete failed";
                }
                else
                {
                    Response.StatusCode = 200;
                    return (string)"Exam Deleted Successfully";
                }
            }

        }
    }
}