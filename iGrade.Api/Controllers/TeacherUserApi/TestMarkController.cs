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

namespace iGrade.Api.Controllers.SchoolApi
{
    [Route("api/s/[controller]")]
    [ApiController]
    public class TestMarkController : BaseUser
    { 
        private TestMarkService _testMarkService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _testMarkService = new TestMarkService(_user , _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet("{id}/test")]
        public ActionResult<object> GetTestMarkListByTestId(Guid id)
        {
            try
            {
                Init();
                return _testMarkService.GetTestMarkListByTestId(id ,ref _sbError);
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]TestMark test)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid && test != null)
                {
                    Response.StatusCode = 400;
                    return "Failed getting test id";
                }
                else
                {
                    var listTestMarks = new List<TestMark>();
                    listTestMarks.Add(test);

                    List<string> error = new List<string>();
                    List<string> success = new List<string>();
                    _testMarkService.Save(listTestMarks, test.TestID, ref error, ref success);
                    if(success.Count >= 1 && error.Count == 0)
                    {
                        return "Record saved";
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return string.Join(" , " , error);
                    }
                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("{testID}/savelist")]
        public ActionResult<object> Save(Guid testID, [FromBody]List<Domain.Form.TestMarkSaveFORM> listTestMarks)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid && listTestMarks != null)
                {
                    Response.StatusCode = 400;
                    return "Failed getting test id";
                }
                else
                {
                    
                    List<string> error = new List<string>();
                    List<string> success = new List<string>();

                   int recordSaved = _testMarkService.SaveBulkByRegNumber(listTestMarks, testID, ref error, ref success);
                    return new { success = recordSaved, error = error };
                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("Delete")]
        public ActionResult<object> DeleteTestMark([FromBody]TeacherUserApi.Model.DeleteTestMark deleteTestMark)
        {
            Init();
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return "TestMark does not exist" ;
            }
            else
            {

                if (deleteTestMark.studentTermRegisterID == null)
                {
                    Response.StatusCode = 400;
                    return "TestMark does not exist";
                }
                StringBuilder sbError = new StringBuilder("");
                List<Guid> list = new List<Guid>();
                list.Add(deleteTestMark.studentTermRegisterID);

                int numberOfRecordsDeleted = 0;
                var isDeleted = _testMarkService.SoftDeleteMark(list, deleteTestMark.testId, ref numberOfRecordsDeleted ,ref sbError);

                if (!isDeleted)
                {
                    Response.StatusCode = 400;
                    return "TestMark Delete failed";
                }
                else
                {
                    Response.StatusCode = 200;
                    return (string)"TestMark Deleted Successfully";
                }
            }
        }
    }
}