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
    public class ClassTeacherController : BaseUser
    { 
        private ClassTeacherService _classTeacherService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
                
            _repository = new UowRepository();
            _classTeacherService = new ClassTeacherService(_user , _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet("CurrentTerm")]
        public ActionResult<object> GetList()
        {
            try
            {
                Init();
                return _classTeacherService.GetListClassTeacherByCurrentTermId(ref _sbError);
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("{id}/term")]
        public ActionResult<object> GetListByTerm(Guid id)
        {
            try
            {
                Init();
                return _classTeacherService.GetListClassTeacherByTermId(id ,ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]ClassTeacher classTeacher)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid || classTeacher == null)
                {
                    Response.StatusCode = 400;
                    return "Failed getting classTeacher id" ;
                }
                else
                {
                    var list = new List<ClassTeacher>();
                    list.Add(classTeacher);
                    var isSaved = _classTeacherService.Save(list, ref sbError);
                    if (isSaved  <= 0)
                    {
                        Response.StatusCode = 400;
                        return "classTeacher save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return (string)"classTeacher saved successfully";
                    }

                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("Delete")] 
        public ActionResult<object> DeleteClassTeacher([FromBody]Model.DeleteClass delClass)
        {
            try
            {
                Init();
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "ClassTeacher does not exist" ;
                }
                else
                {
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _classTeacherService.Delete(delClass.classId, delClass.teacherId, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return "ClassTeacher Delete failed" ;
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"ClassTeacher Deleted Successfully";
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