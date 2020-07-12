using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iGrade.Api.Controllers.TeacherUserApi.Model;
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
    public class AbsentFromSchoolController : BaseUser
    {
        private AbsentFromSchoolService _absentFromSchoolService;
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _absentFromSchoolService = new AbsentFromSchoolService(_user, _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet("classCurrentTerm")]
        public ActionResult<object> GetList(Guid classid)
        {
            try
            {
                Init();
                return _absentFromSchoolService.GetListByClassIdAndTermId(classid , _user.TermID , ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }
        
        [HttpPost]
        public ActionResult<object> Save([FromBody]AbsentFromSchoolForm schoolForm)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid || schoolForm == null)
                {
                    Response.StatusCode = 400;
                    return "Please fill in all fields";
                }
                else
                {
                    
                    var isSaved = _absentFromSchoolService.Save( schoolForm.StudentTermRegisterID , schoolForm.DayAbsent , schoolForm.Reason , ref sbError);
                    if (!isSaved)
                    {
                        Response.StatusCode = 400;
                        return "absent record not save failed " + sbError.ToString();
                    }
                    else
                    {
                        return (string)"absent saved successfully";
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
                    return "Absent does not exist"
                    ;
                }
                else
                {

                    if (deleteID == null)
                    {
                        Response.StatusCode = 400;
                        return "Absent does not exist"
                        ;
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _absentFromSchoolService.Delete(deleteID.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return "Absent Delete failed"
                        ;
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"Absent Deleted Successfully";
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