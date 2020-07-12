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
    public class TeacherClassSubjectFileTypeController : BaseUser
    {
        private TeacherClassSubjectFileTypeService _TeacherClassSubjectFileTypeService;
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _TeacherClassSubjectFileTypeService = new TeacherClassSubjectFileTypeService(_user, _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet]
        public ActionResult<object> GetList()
        {
            try
            {
                Init();
                return _TeacherClassSubjectFileTypeService.GetTeacherClassSubjectFileTypeBySchoolId();
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
                    return _TeacherClassSubjectFileTypeService.GetTeacherClassSubjectFileType((Guid)id);
                }
                return null;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]TeacherClassSubjectFileType teacherClassSubjectFileType)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return (string)"Failed getting teacherClassSubjectFileType id" ;
                }
                else
                {
                    if (string.IsNullOrEmpty(teacherClassSubjectFileType?.TeacherClassSubjectFileTypeId.ToString()))
                    {
                        teacherClassSubjectFileType.SchoolId = _user.SchoolID;
                    }
                    var isSaved = _TeacherClassSubjectFileTypeService.Save(teacherClassSubjectFileType, ref sbError);
                    if (isSaved == null)
                    {
                        Response.StatusCode = 400;
                        return (string)"teacherClassSubjectFileType save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return Ok(new { id = isSaved.TeacherClassSubjectFileTypeId });
                    } 
                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("Delete")]
        public ActionResult<object> DeleteTeacherClassSubjectFileType([FromBody]Model.DeleteID deleteID)
        {
            try
            { 
                Init();
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "TeacherClassSubjectFileType does not exist"  ;
                }
                else
                {
                    if (deleteID == null)
                    {
                        Response.StatusCode = 400;
                        return "TeacherClassSubjectFileType does not exist" ;
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _TeacherClassSubjectFileTypeService.Delete(deleteID.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return  "TeacherClassSubjectFileType Delete failed";
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"TeacherClassSubjectFileType Deleted Successfully";
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