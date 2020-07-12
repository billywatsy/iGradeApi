using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using iGrade.Core.TeacherUserService;
using iGrade.Domain;
using iGrade.Domain.Dto;
using iGrade.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGrade.Api.Controllers.TeacherUserApi
{
    [Route("api/s/[controller]")]
    [ApiController]
    public class StudentTermReviewController : BaseUser
    { 
        private StudentTermReviewService _studentTermReviewService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession(); 
            _repository = new UowRepository();
            _studentTermReviewService = new StudentTermReviewService(_user , _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet("teacherSearch")]
        public ActionResult<object> GetTeacherSearch(DateTime? dateYearMonth)
        {
            try
            {
                Init();
                if (dateYearMonth == null) {
                    dateYearMonth = DateTime.Now;
                };
                return _studentTermReviewService.GetListByTeacherReviewsByYearAndMonth(_user.TeacherID , (DateTime)dateYearMonth) ?? new List<StudentTermReviewDto>();
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }
        
        [HttpPost]
        public ActionResult<object> Save([FromBody]Domain.Form.StudentTermReviewForm review)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return  "Failed getting review" ;
                }
                else
                {
                    
                    var isSaved = _studentTermReviewService.Save(review, ref sbError);
                    if (!isSaved)
                    {
                        Response.StatusCode = 400;
                        return "review save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return (string)"review saved successfully";
                    }

                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("Delete")]
        public ActionResult<object> Delete([FromBody] Model.DeleteID deleteID)
        {
            try
            {
                Init();
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "review request is wrong ";
                }
                else
                {

                    if (deleteID.id == null)
                    {
                        Response.StatusCode = 400;
                        return  "review does not exist" ;
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted =  _studentTermReviewService.Delete(deleteID.id);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return  "review Delete failed" ;
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"review Deleted Successfully";
                    }
                }
            }
            catch (Exception er)
            {
                return Error(er);
            }

        }
    }
}