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
    public class LevelController : BaseUser
    {
        private LevelService _levelService;
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _levelService = new LevelService(_user, _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet]
        public ActionResult<object> GetList()
        {
            try
            {
                Init();
                return _levelService.GetListLevelBySchoolID(ref _sbError);
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
                    return _levelService.GetLevelByLevelId((Guid)id, ref _sbError);
                }
                return null;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]Level level)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return (string)"Failed getting level id" ;
                }
                else
                {
                    if (string.IsNullOrEmpty(level.LevelID.ToString()))
                    {
                        level.SchoolID = _user.SchoolID;
                    }
                    var isSaved = _levelService.Save(level, ref sbError);
                    if (isSaved == null)
                    {
                        Response.StatusCode = 400;
                        return (string)"level save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return Ok(new { id = isSaved.LevelID });
                    } 
                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("Delete")]
        public ActionResult<object> DeleteLevel([FromBody]Model.DeleteID deleteID)
        {
            try
            { 
                Init();
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "Level does not exist"  ;
                }
                else
                {

                    if (deleteID == null)
                    {
                        Response.StatusCode = 400;
                        return "Level does not exist" ;
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _levelService.Delete(deleteID.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return  "Level Delete failed";
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"Level Deleted Successfully";
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