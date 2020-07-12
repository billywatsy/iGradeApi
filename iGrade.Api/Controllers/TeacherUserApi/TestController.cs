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
    public class TestController : BaseUser
    { 
        private TestService _testService;  
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _testService = new TestService(_user , _repository); 
            _sbError = new StringBuilder("");
        }

        [HttpGet("{id}/teacherclasssubject")]
        public ActionResult<object> GetListTestByTeacherClassSubjectID(Guid id)
        {
            try
            {
                Init();
                return _testService.GetListTestDtoByTeacherClassSubjectID(id ,ref _sbError);
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
                    return _testService.GetTestByTestId((Guid)id, ref _sbError);
                }
                return null;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }



         


        [HttpPost]
        public ActionResult<object> Save([FromBody]Test test)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "Failed getting test id";
                }
                else
                {
                    
                    var isSaved = _testService.Save(test, ref sbError);
                    if (!isSaved)
                    {
                        Response.StatusCode = 400;
                        return "test save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return (string)"test saved successfully";
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
            Init();
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return new string[]
                { // to
                   "Test does not exist"
                };
            }
            else
            {

                if (deleteID.id == null)
                {
                    Response.StatusCode = 400;
                    return new string[]
                    { // to
                      "Test does not exist"
                    };
                }
                StringBuilder sbError = new StringBuilder("");

                var isDeleted = _testService.Delete(deleteID.id, ref sbError);

                if (!isDeleted)
                {
                    Response.StatusCode = 400;
                    return new string[]
                    { // to
                       "Test Delete failed"
                    };
                }
                else
                {
                    Response.StatusCode = 200;
                    return (string)"Test Deleted Successfully";
                }
            }

        }
    }
}