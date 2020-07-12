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
    public class SchoolInformationController : BaseUser
    { 
        private SchoolInformationService _schoolInformationService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _schoolInformationService = new SchoolInformationService(_user , _repository);
            _sbError = new StringBuilder("");
        }
         
        [HttpGet]
        public ActionResult<object> Get()
        {
            try
            {
                Init();
                return _schoolInformationService.GetSchoolInfo();
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]SchoolInformation schoolInformation)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "Failed getting schoolInformation id" ;
                }
                else
                {
                    if (string.IsNullOrEmpty(schoolInformation.SchoolId.ToString()))
                    {
                        schoolInformation.SchoolId = _user.SchoolID;
                    }
                    var isSaved = _schoolInformationService.Save(schoolInformation, ref sbError);
                    if (!isSaved)
                    {
                        Response.StatusCode = 400;
                        return "schoolInformation save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return (string)"schoolInformation saved successfully";
                    }

                }
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }
        
    }
}