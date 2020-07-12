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
    public class ClassController : BaseUser
    {
        private ClassService _classService;
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _classService = new ClassService(_user, _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet]
        public ActionResult<object> GetList()
        {
            try
            {
                Init();
                return _classService.GetClassesBySchoolId(ref _sbError);
            }
            catch (Exception er)
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
                    return _classService.GetClassById((Guid)id, ref _sbError);
                }
                return null;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]Class scool_class)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "Failed getting class id";
                }
                else
                {
                    if (string.IsNullOrEmpty(scool_class.ClassID.ToString()))
                    {
                        scool_class.SchoolID = _user.SchoolID;
                    }
                    var isSaved = _classService.Save(scool_class, ref sbError);
                    if (isSaved == null)
                    {
                        Response.StatusCode = 400;
                        return "class save failed " + sbError.ToString();
                    }
                    else
                    {
                        return Ok(new { id = isSaved.ClassID });
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
                    return "Class does not exist"
                    ;
                }
                else
                {

                    if (deleteID.id == null)
                    {
                        Response.StatusCode = 400;
                        return "Class does not exist"
                        ;
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _classService.Delete(deleteID.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return "Class Delete failed"
                        ;
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"Class Deleted Successfully";
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