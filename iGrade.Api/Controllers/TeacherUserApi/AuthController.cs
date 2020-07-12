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
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGrade.Api.Controllers.TeacherUserApi
{
    [Route("api/s/[controller]")]
    [ApiController]
   // [EnableCors("AllowAll")]
    public class AuthController : ControllerBase
    { 
        private AuthService _authService;   
        private StringBuilder _sbError;

        public void Init()
        {  
            _authService = new AuthService();
            _sbError = new StringBuilder("");
        }

        [HttpPost]
        public ActionResult<object> Login([FromBody]Login login)
        {
            try
            {
                Init();
                var user = _authService.Login(login.Username, login.Password, ref _sbError);

                if(user == null)
                { 
                    return new BadRequestObjectResult(_sbError.ToString());
                }
                return new {
                                access_token = user.WebToken ,
                                name = user.TeacherFullname ,
                                classId = user.ClassID , 
                                isAdmin = user.isAdmin ,
                                termDetails = "Year :"+user.CurrentTermYear + " Number :" + user.CurrentTermNumber , 
                                schoolName  = user.SchoolName , 
                                schoolCode = user.SchoolCode 
                };
            }
            catch(Exception er)
            {
                return new BadRequestResult();
            }
        }
        [HttpGet("RequestTokenAndSendEmailToken")]
        public ActionResult<object> RequestTokenAndSendEmailToken(string username , string email)
        {
            try
            {
                Init();
                var isSend = _authService.RequestNewForgotPassword(email, username , ref _sbError);

                if (isSend)
                {
                    return Ok();
                }
                return BadRequest(_sbError.ToString());
            }
            catch(Exception er)
            {
                return BadRequest("Error processig request ");
            }
        }

        [HttpPost("ChangePasswordByToken")]
        public ActionResult<object> ChangePasswordByToken([FromBody]ForgotPasswordForm form)
        {
            try
            {
                Init();
                var ischanged = _authService.ChangePasswordForgot(form.ForgotPasswordToken , form.NewPassword , form.ConfirmPassword , ref _sbError);
                if (ischanged)
                {
                    return Ok();
                }
                return BadRequest(_sbError.ToString());
            }
            catch (Exception er)
            {
                return BadRequest("Error message");
            }
        }

    }
}