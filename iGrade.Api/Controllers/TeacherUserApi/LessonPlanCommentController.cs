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
    public class LessonPlanCommentController : BaseUser
    {
        private LessonPlanCommentService _lessonPlanCommentService;
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _lessonPlanCommentService = new LessonPlanCommentService(_user, _repository);
            _sbError = new StringBuilder("");
        }
        
        [HttpGet("{id}/LessonPlan")]
        public ActionResult<object> GetListByLessonPlanID(Guid id)
        {
            try
            {
                Init();
                return _lessonPlanCommentService.GetListByLessonPlanID(id, ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        
        [HttpPost]
        public ActionResult<object> Save([FromBody]LessonPlanComment lessonPlanComment)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return (string)"Failed getting level id" ;
                }
                else
                {
                    var isSaved = _lessonPlanCommentService.Save(lessonPlanComment, ref sbError);
                    if (isSaved == null)
                    {
                        Response.StatusCode = 400;
                        return (string)"level save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return Ok(isSaved);
                    } 
                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("Delete")]
        public ActionResult<object> Delete([FromBody]Model.DeleteID deleteID)
        {
            try
            { 
                Init();
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "comment does not exist"  ;
                }
                else
                {

                    if (deleteID == null)
                    {
                        Response.StatusCode = 400;
                        return "comment does not exist";
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _lessonPlanCommentService.Delete(deleteID.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return "comment Delete failed";
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"comment Deleted Successfully";
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