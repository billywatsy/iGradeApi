using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iGrade.Api.Controllers.ParentApi.Model;
using iGrade.Domain.Dto;
using iGrade.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGrade.Api.Controllers.ParentApi
{
    [Route("api/p/[controller]")]
    [ApiController]
    public class AuthController :  BaseParent
    { 
        private UowRepository _repository;
        private ParentSessionDto _user;
        private StringBuilder _sbError;
        private Core.ParentService.AuthService _authService;
        public void Init()
        {
            _repository = new UowRepository(); 
            _sbError = new StringBuilder("");
            _authService = new Core.ParentService.AuthService();
        }

        [HttpPost("login")]
        public ActionResult<object> Login([FromBody]LoginParent login)
        {
            try
            {
                Init();

                var token = _authService.Login(login?.Email, login?.SchoolCode, login?.Password, ref _sbError);

                if (string.IsNullOrEmpty(token?.Token))
                {
                    Response.StatusCode = 400;
                    return (string)_sbError.ToString();
                }
                return new { token = token?.Token };
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }



        [HttpPost("register")]
        public ActionResult<object> Register([FromBody]LoginParent login)
        {
            try
            {
                Init();

                var token = _authService.Register(login?.Email, login?.SchoolCode, ref _sbError);

                if (!token)
                {
                    Response.StatusCode = 400;
                    return (string)_sbError.ToString();
                }
                return true;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("sendOneTimePin")]
        public ActionResult<object> SendOneTimePin([FromBody]LoginParent login)
        {
            try
            {
                Init();

                var token = _authService.SendMeOneTimePin(login?.Email, login?.SchoolCode, ref _sbError);

                if (!token)
                {
                    Response.StatusCode = 400;
                    return (string)_sbError.ToString();
                }
                return true;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

    }
}
