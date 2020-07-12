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
    public class TermController : BaseUser
    {
        private TermService _termService;
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _termService = new TermService(_user, _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet]
        public ActionResult<object> GetList()
        {
            try
            {
                Init();
                return _termService.GetListBySchoolID(ref _sbError);
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
                    return _termService.GetTermId((Guid)id, ref _sbError);
                }
                return null;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("active")]
        public ActionResult<object> GetActiveTerm()
        {
            try
            {
                Init();
                 return _termService.GetTermId(_user.TermID , ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }



        [HttpPost]
        public ActionResult<object> Save([FromBody]Term term)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "Failed getting term id" ;
                }
                else
                {
                    if (string.IsNullOrEmpty(term.TermID.ToString()))
                    {
                        term.SchoolID = _user.SchoolID;
                    }
                    var isSaved = _termService.Save(term, ref sbError);
                    if (isSaved == null)
                    {
                        Response.StatusCode = 400;
                        return "term save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return Ok(new { id = isSaved.TermID });
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
                    return "Term does not exist"
                    ;
                }
                else
                {

                    if (deleteID.id == null)
                    {
                        Response.StatusCode = 400;
                        return "Term does not exist";
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _termService.Delete(deleteID.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return "Term Delete failed";
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"Term Deleted Successfully";
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