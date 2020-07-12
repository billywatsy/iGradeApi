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
    public class TeacherDepartmentController : BaseUser
    { 
        private TeacherDepartmentService _teacherDepartmentService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
                
            _repository = new UowRepository();
            _teacherDepartmentService = new TeacherDepartmentService(_user , _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet]
        public ActionResult<object> GetList()
        {
            try
            {
                Init();
                return _teacherDepartmentService.GetListBySchoolID(ref _sbError);
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("{id}/Department")]
        public ActionResult<object> GetListByDepartmentID(Guid id)
        {
            try
            {
                Init();
                return _teacherDepartmentService.GetListByDepartmentID(id, ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpGet("{id}/Teacher")]
        public ActionResult<object> GetListByTeacherID(Guid id)
        {
            try
            {
                Init();
                return _teacherDepartmentService.GetListByTeacherID(id, ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]TeacherDepartment teacherDepartment)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid || teacherDepartment == null)
                {
                    Response.StatusCode = 400;
                    return "Failed getting teacherDepartment id" ;
                }
                else
                {
                    var isSaved = _teacherDepartmentService.Save(teacherDepartment, ref sbError);
                    if (!isSaved )
                    {
                        Response.StatusCode = 400;
                        return "teacherDepartment save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return (string)"teacherDepartment saved successfully";
                    }

                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("Delete")] 
        public ActionResult<object> DeleteTeacherDepartment([FromBody]Model.DeleteID delClass)
        {
            try
            {
                Init();
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "Teacher Department does not exist" ;
                }
                else
                {
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _teacherDepartmentService.Delete(delClass.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return "Teacher Department Delete failed" ;
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"Teacher Department Deleted Successfully";
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