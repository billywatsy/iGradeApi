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
    public class DepartmentController : BaseUser
    {
        private DepartmentService _departmentService;
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _departmentService = new DepartmentService(_user, _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet]
        public ActionResult<object> GetList()
        {
            try
            {
                Init();
                return _departmentService.GetDepartments();
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
                    return _departmentService.GetDepartment((Guid)id);
                }
                return null;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]Department department)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return (string)"Failed getting department id" ;
                }
                else
                {
                    if (string.IsNullOrEmpty(department?.DepartmentId?.ToString()))
                    {
                        department.SchoolID = _user.SchoolID;
                    }
                    var isSaved = _departmentService.Save(department, ref sbError);
                    if (isSaved == null)
                    {
                        Response.StatusCode = 400;
                        return (string)"department save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return Ok(new { id = isSaved.DepartmentId });
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
                    return "Department does not exist"  ;
                }
                else
                {

                    if (deleteID == null)
                    {
                        Response.StatusCode = 400;
                        return "Department does not exist" ;
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _departmentService.Delete(deleteID.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return  "Department Delete failed";
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"Department Deleted Successfully";
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