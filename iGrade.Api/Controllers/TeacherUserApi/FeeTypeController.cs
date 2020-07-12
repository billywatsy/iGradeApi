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
    public class FeeTypeController : BaseUser
    {
        private FeeTypeService _feeTypeService;
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _feeTypeService = new FeeTypeService(_user, _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet]
        public ActionResult<object> GetList()
        {
            try
            {
                Init();
                return _feeTypeService.GetListBySchoolID(ref _sbError);
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
                    return _feeTypeService.GetByID((Guid)id, ref _sbError);
                }
                return null;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]FeeType feeType)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return (string)"Failed getting feeType id" ;
                }
                else
                {
                    if (string.IsNullOrEmpty(feeType.FeeTypeID.ToString()))
                    {
                        feeType.SchoolID = _user.SchoolID;
                    }
                    var isSaved = _feeTypeService.Save(feeType, ref sbError);
                    if (!isSaved)
                    {
                        Response.StatusCode = 400;
                        return (string)"feeType save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return Ok(true);
                    } 
                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("Delete")]
        public ActionResult<object> DeleteFeeType([FromBody]Model.DeleteID deleteID)
        {
            try
            { 
                Init();
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "FeeType does not exist"  ;
                }
                else
                {

                    if (deleteID == null)
                    {
                        Response.StatusCode = 400;
                        return "FeeType does not exist" ;
                    }
                    StringBuilder sbError = new StringBuilder("");

                    var isDeleted = _feeTypeService.Delete(deleteID.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return  "FeeType Delete failed";
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"FeeType Deleted Successfully";
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