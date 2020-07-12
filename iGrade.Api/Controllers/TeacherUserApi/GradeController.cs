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
    public class GradeController : BaseUser
    { 
        private GradeService _gradeService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _gradeService = new GradeService(_user , _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet]
        public ActionResult<object> GetList()
        {
            try
            {
                Init();
                return _gradeService.GetListBySchoolID( ref _sbError);
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
                    return _gradeService.GetByID((Guid)id, ref _sbError);
                }
                return null;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]Grade grade)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "Failed getting grade id" ;
                }
                else
                {
                    if (string.IsNullOrEmpty(grade.GradeID.ToString()))
                    {
                        grade.SchoolID = _user.SchoolID;
                    }
                    var isSaved = _gradeService.Save(grade, ref sbError);
                    if (isSaved == null)
                    {
                        Response.StatusCode = 400;
                        return "grade save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return Ok(new { id = isSaved.GradeID });
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
                    return "Grade does not exist" ;
                }
                else
                {

                    if (deleteID.id == null)
                    {
                        Response.StatusCode = 400;
                        return "Grade does not exist"  ;
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _gradeService.Delete(deleteID.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return  "Grade Delete failed" ;
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"Grade Deleted Successfully";
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