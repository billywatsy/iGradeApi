using iGrade.Core.TeacherUserService;
using iGrade.Domain;
using iGrade.Domain.Dto;
using iGrade.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iGrade.Api.Controllers
{
    [Route("api/s/[controller]")]
    [ApiController]
    public class LogController : BaseUser
    {
        private LogService _logService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository(); 
            _logService = new LogService(_user, _repository); 
            _sbError = new StringBuilder("");
        }

        [HttpGet("all")]
        public ActionResult<object> Search(int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                Init();
                return _logService.GetPagedList(pageSize, pageNumber, ref _sbError);
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("teacher")]
        public ActionResult<object> Teacher(int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                Init();
                return _logService.GetPagedListTeacher(pageSize, pageNumber, ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }
        
    }
}