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
    public class TeacherController : BaseUser
    { 
        private TeacherService _teacherService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _repository = new UowRepository();
            StringBuilder sbError = new StringBuilder("");
            _user = UserSession();
            _teacherService = new TeacherService(_user , _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet]
        public ActionResult<object> Search(int pageSize = 10, int pageNumber = 1, string q = null)
        {
            try
            {
                Init();
                PagedList<TeacherDto> data = _teacherService.GetPagedListTeacherSearch(pageSize, pageNumber, q, ref _sbError) ?? new PagedList<TeacherDto>();
                return data;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("isActive")]
        public ActionResult<object> GetActiveTeachers()
        {
            try
            {
                Init();
                List<Teacher> data = _teacherService.GetTeachersBySchoolIdAndActiveStatus(true, ref _sbError) ?? new List<Teacher>();
                return data;
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
                    return _teacherService.GetTeacherByTeacherId((Guid)id, ref _sbError);
                }
                return null;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("profile")]
        public ActionResult<object> Profile()
        {
            try
            {
                Init();
                return Get((Guid)_user.TeacherID);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("changePassword")]
        public ActionResult<object> ChangePassword([FromBody] Model.ChangePasswordForm form)
        {
            try
            {
                Init();
                if(form == null)
                {
                    return new BadRequestObjectResult("Please fill in all fields");
                }
                bool changePassword = new AuthService().ChangePassword(_user.Username, form.OldPassword, form.NewPassword, form.ConfirmPassword, ref _sbError);

                if (changePassword)
                {
                    return Ok();
                }
                return BadRequest("Error changing Password or "+ _sbError.ToString());
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost]
        public ActionResult<object> Save([FromBody]Teacher teacher)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "Failed getting teacher id" ;
                }
                else
                {
                    if (string.IsNullOrEmpty(teacher.TeacherID.ToString()))
                    {
                        teacher.SchoolID = _user.SchoolID;
                    }
                    var isSaved = _teacherService.Save(teacher, ref sbError);
                    if (isSaved == null)
                    {
                        Response.StatusCode = 400;
                        return  "teacher save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return Ok(new { id = isSaved.TeacherID });
                    }

                }
            }
            catch(Exception er)
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
                    return "Teacher does not exist";
                }
                else
                {

                    if (deleteID.id == null)
                    {
                        Response.StatusCode = 400;
                        return "Teacher does not exist";
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _teacherService.Delete(deleteID.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return "Teacher Delete failed";
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"Teacher Deleted Successfully";
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