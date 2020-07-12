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
    public class GradeMarkController : BaseUser
    { 
        private GradeMarkService _gradeMarkService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _gradeMarkService = new GradeMarkService(_user , _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet("{id}/grade")]
        public ActionResult<object> GetList(Guid id)
        {
            try
            {
                Init();
                return _gradeMarkService.GetListByGradeID(id , ref _sbError);
            }
            catch
            {
                return null;
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
                    return _gradeMarkService.GetByID((Guid)id, ref _sbError);
                }
                return null;
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]GradeMark gradeMark)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return new string[] { "Failed getting gradeMark id" };
                }
                else
                {
                    
                    var isSaved = _gradeMarkService.Save(gradeMark, ref sbError);
                    if (isSaved == null)
                    {
                        Response.StatusCode = 400;
                        return new string[] { "gradeMark save failed " + sbError.ToString() };
                    }
                    else
                    {
                        return Ok(new { id = isSaved .GradeMarkID });
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
            Init();
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return    "GradeMark does not exist"
                ;
            }
            else
            {

                if (deleteID.id == null)
                {
                    Response.StatusCode = 400;
                    return   "GradeMark does not exist"
                    ;
                }
                StringBuilder sbError = new StringBuilder("");

                var isDeleted = _gradeMarkService.Delete(deleteID.id, ref sbError);

                if (!isDeleted)
                {
                    Response.StatusCode = 400;
                    return    "GradeMark Delete failed"
                    ;
                }
                else
                {
                    Response.StatusCode = 200;
                    return (string)"GradeMark Deleted Successfully";
                }
            }

        }
    }
}