using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iGrade.Core.TeacherUserService;
using iGrade.Domain;
using iGrade.Domain.Dto;
using iGrade.Domain.Form;
using iGrade.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGrade.Api.Controllers.TeacherUserApi
{
    [Route("api/s/[controller]")]
    [ApiController]
    public class StudentTermRegisterController : BaseUser
    { 
        private StudentTermRegisterService _studentTermRegisterService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _studentTermRegisterService = new StudentTermRegisterService(_user , _repository);
            _sbError = new StringBuilder("");
        }


        // classs id
        [HttpGet("currentTerm")]
        public ActionResult<object> Get()
        {
            try
            {
                Init();
                return _studentTermRegisterService.GetListBySchoolIDAndTermID(_user.TermID, ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        // classs id
        [HttpGet("Term")]
        public ActionResult<object> GetListByTermId(Guid? termId)
        {
            try
            {
                Init();
                if (termId == null)
                    termId = _user.TermID;
                return _studentTermRegisterService.GetListBySchoolIDAndTermID((Guid)termId, ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpGet("{id}/studentInClassByTestID")]
        public ActionResult<object> GetStudentInClassByTestID(Guid? id)
        {
            try
            {
                Init();
                if (id != null && Guid.Empty != id)
                {
                    return _studentTermRegisterService.GetStudentsByTestId((Guid)id, ref _sbError);
                }
                return null;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }




       


        [HttpGet("{id}/studentInClassByTeacherClassSubjectID")]
        public ActionResult<object> GetStudentInClassByTeacherClassSubjectID(Guid id)
        {
            try
            {
                Init();
                return _studentTermRegisterService.GetListByTeacherClassSubjectID(id, ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpGet("{id}/currentClass")]
        public ActionResult<object> GetCurrentClassID(Guid id)
        {
            try
            {
                Init();
                return _studentTermRegisterService.GetClassListByClassIDAndSchoolIDAndTermID(id , _user.TermID ,  ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("{id}/id")]
        public ActionResult<object> GetById(Guid id)
        {
            try
            {
                Init();
                return _studentTermRegisterService.GetByID(id, ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save(StudentTermRegisterFORM save)
        {
            try
            {
                Init();
                var isSaved = _studentTermRegisterService.SingleSave(save, ref _sbError);

                if (isSaved == null)
                {
                    Response.StatusCode = 400;
                    return "Error " + _sbError.ToString();
                }
                return true;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost("savelist")]
        public ActionResult<object> SaveList(List<ExcelPostListDto> save)
        {
            try
            {
                Init();
                var isSaved = _studentTermRegisterService.SaveList(save);
                
                return isSaved;
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
                    return "review request is wrong ";
                }
                else
                {

                    if (deleteID.id == null)
                    {
                        Response.StatusCode = 400;
                        return "review does not exist";
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _studentTermRegisterService.Delete(deleteID.id , ref _sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return "review Delete failed";
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"review Deleted Successfully";
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