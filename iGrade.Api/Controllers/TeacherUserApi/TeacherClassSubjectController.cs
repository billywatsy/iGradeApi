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
    public class TeacherClassSubjectController : BaseUser
    { 
        private TeacherClassSubjectService _teacherClassSubjectService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _teacherClassSubjectService = new TeacherClassSubjectService(_user , _repository);
            _sbError = new StringBuilder("");
        }
        
        [HttpGet("currentSubjects")]
        public ActionResult<object> GetList()
        {
            try
            {
                Init();
                return _teacherClassSubjectService.GetCurrentTermTeacherClassSubjects( ref _sbError);
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }


        [HttpGet("currentallTermSubjects")]
        public ActionResult<object> AllTermSubjects()
        {
            try
            {
                Init();

                return _teacherClassSubjectService.GetListTeacherClassSubjectsBySchoolIDandTermID(_user.TermID, ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpGet("{id}/term")]
        public ActionResult<object> GetByTerm(Guid id)
        {
            try
            {
                Init();
                return _teacherClassSubjectService.GetListTeacherClassSubjectsBySchoolIDandTermID(id, ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<object> GetById(Guid id)
        {
            try
            {
                Init();
                return _teacherClassSubjectService.GetTeacherClassSubjectsByID(id, ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("HeadOfDepartmentOrAdmin")]
        public ActionResult<object> GetListByHeadOfDepartmentOrAdmin()
        {
            try
            {
                Init();
                return _teacherClassSubjectService.GetListByHeadOfDepartmentOrAdmin(ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }
         

        [HttpPost("CreateByClassList")]
        public ActionResult<object> CreateByClassList(TeacherClassSubjectSaveMethodClassListFORM formClassList)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder();
                int isUpdated = _teacherClassSubjectService.SaveByTeacherClassSubject(formClassList, null, ref sbError);

                if (isUpdated > 0)
                {
                    return (string)"Teacher Class Subject Saved ";
                }
                Response.StatusCode = 400;
                return sbError.ToString();
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("CreateBySubjectList")]
        public ActionResult<object> CreateBySubjectList(TeacherClassSubjectSaveMethodSubjectListFORM formSubjectList)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder();
                int isUpdated = _teacherClassSubjectService.SaveByTeacherClassSubject(null, formSubjectList, ref sbError);

                if (isUpdated > 0)
                {
                    return (string)"Teacher Class Subject Saved ";
                }
                Response.StatusCode = 400;
                return sbError.ToString();
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
                    return "id does not exist";
                }
                else
                {

                    if (deleteID.id == null)
                    {
                        Response.StatusCode = 400;
                        return "id  does not exist";
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _teacherClassSubjectService.Delete(deleteID.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return "id Delete failed / "+ sbError.ToString();
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"id Deleted Successfully";
                    }
                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("ImportTerm")]
        public ActionResult<object> ImportStepSave(Guid ToTermID, Guid FromTermID)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");
                List<int> ltSuccess = new List<int>();
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "Please fill in correct information";
                }

                var saved = _teacherClassSubjectService.ImportTeacherClassSubjectFromTermToAnotherTerm(FromTermID, ToTermID, ref sbError);

                return (string)(saved + $" records saved successfully and [{ sbError.ToString()}]");
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }
    }
}