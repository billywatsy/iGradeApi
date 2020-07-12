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
    public class SubjectController : BaseUser
    { 
        private SubjectService _subjectService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession(); 
            _repository = new UowRepository();
            _subjectService = new SubjectService(_user , _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet]
        public ActionResult<object> GetList()
        {
            try
            {
                Init();
                return _subjectService.GetSubjects();
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }
        [HttpGet("search")]
        public ActionResult<object> Search([FromQuery]UserSearch search)
        {
            try
            {
                Init();
                if(search == null || string.IsNullOrEmpty(search?.Q))
                {
                    var subjects = _subjectService.GetSubjects();
                    return ListToPage<Subject>(subjects, search.Size, search.Page);
                }
                else
                {
                    var subjects = _subjectService.GetSubjects()?.Where(c => c.SubjectCode.Contains(search.Q) ||
                                                                           c.SubjectName.Contains(search.Q))?.ToList();
                    return ListToPage<Subject>(subjects, search.Size, search.Page);

                }
               
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
                    return _subjectService.GetSubject((Guid)id);
                }
                return null;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]Subject subject)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return  "Failed getting subject id" ;
                }
                else
                {
                    
                    var isSaved = _subjectService.Save(subject, ref sbError);
                    if (isSaved == null)
                    {
                        Response.StatusCode = 400;
                        return "subject save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return Ok(new { id = isSaved.SubjectID });
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
                    return "Subject does not exist";
                }
                else
                {

                    if (deleteID.id == null)
                    {
                        Response.StatusCode = 400;
                        return  "Subject does not exist" ;
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _subjectService.Delete(deleteID.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return  "Subject Delete failed" ;
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"Subject Deleted Successfully";
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