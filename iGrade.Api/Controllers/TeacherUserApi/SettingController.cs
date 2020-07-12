using iGrade.Core.TeacherUserService;
using iGrade.Domain;
using iGrade.Domain.Dto;
using iGrade.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iGrade.Api.Controllers.TeacherUserApi.Model;

namespace iGrade.Api.Controllers
{
    [Route("api/s/[controller]")]
    [ApiController]
    public class SettingController : BaseUser
    {
        private SettingService _settingService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _settingService = new SettingService(_user, _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet]
        public ActionResult<object> GetCurrentSetting()
        {
            try
            {
                Init();
                List<string> eror = new List<string>();
                var settings =  _settingService.GetSchoolSetting(_user.SchoolID, ref eror);

                
                return settings;
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]Setting form)
        {
            try
            {
                Init();
                List<string> sbError = new List<string>();

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "Failed getting student id";
                }
                else
                {
                    var studentSaved = _settingService.Save(form, ref sbError);
                    if (studentSaved == null)
                    {
                        Response.StatusCode = 400;
                        return "student save failed " + string.Join(" , ", sbError);
                    }
                    else
                    {
                        return (string)"student saved successfully";
                    }
                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost("SaveExamTestDate")]
        public ActionResult<object> SaveExamTestDate([FromBody]SettingForm form)
        {
            try
            {
                Init();
                List<string> sbError = new List<string>();

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "Failed getting student id";
                }
                else
                {
                    var setting = _settingService.GetSchoolSetting(_user.SchoolID, ref sbError);
                    setting.ExamMarkSubmissionClosingDate = form.Exam;
                    setting.TestMarkSubmissionClosingDate = form.Test;

                    var studentSaved = _settingService.Save(setting, ref sbError);
                    if (studentSaved == null)
                    {
                        Response.StatusCode = 400;
                        return "student save failed " + string.Join(" , ", sbError);
                    }
                    else
                    {
                        return (string)"student saved successfully";
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