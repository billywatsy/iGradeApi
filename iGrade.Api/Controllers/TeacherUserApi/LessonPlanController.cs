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
    public class LessonPlanController : BaseUser
    {
        private LessonPlanService _levelService;
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _levelService = new LessonPlanService(_user, _repository);
            _sbError = new StringBuilder("");
        }



        [HttpGet("{id}")]
        public ActionResult<object> Get(Guid? id)
        {
            try
            {
                Init();
                if (id != null && Guid.Empty != id)
                {
                    return _levelService.GetLessonPlan((Guid)id, ref _sbError);
                }
                return null;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("{id}/teacherclasssubject")]
        public ActionResult<object> GetListLessonPlansByTeacherClassSubjectID(Guid id)
        {
            try
            {
                Init();
                return _levelService.GetListLessonPlansByTeacherClassSubjectID(id, ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]LessonPlan level)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return (string)"Failed getting level id";
                }
                else
                {
                    var isSaved = _levelService.Save(level, ref sbError);
                    if (isSaved == null)
                    {
                        Response.StatusCode = 400;
                        return (string)"level save failed " + sbError.ToString();
                    }
                    else
                    {
                        return Ok(new { id = isSaved });
                    }
                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("Approve")]
        public ActionResult<object> Approve([FromBody]LessonPlan level)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return (string)"Failed getting level id";
                }
                else
                {
                    var isSaved = _levelService.ApproveLessonPlan((Guid)level.LessonPlanId, ref sbError);
                    if (isSaved == null)
                    {
                        Response.StatusCode = 400;
                        return (string)"level save failed " + sbError.ToString();
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
        public ActionResult<object> DeleteLessonPlan([FromBody]Model.DeleteID deleteID)
        {
            try
            { 
                Init();
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "LessonPlan does not exist"  ;
                }
                else
                {

                    if (deleteID == null)
                    {
                        Response.StatusCode = 400;
                        return "LessonPlan does not exist" ;
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _levelService.Delete(deleteID.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return  "LessonPlan Delete failed "+ sbError.ToString();
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"LessonPlan Deleted Successfully";
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