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
    public class DashboardController : BaseUser
    { 
        private DashboardService _dashboardService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        private void Init()
        {
            _user = UserSession(); 
            if(_user != null)
            {
                _repository = new UowRepository();
                _dashboardService = new DashboardService(_user, _repository);
                _sbError = new StringBuilder("");
            }
        }

        [HttpGet]
        public ActionResult<object> Get()
        {
            try
            {
                Init();

                var count_term_data = _dashboardService.GetDashboardCountDto(ref _sbError);
                var list = _dashboardService.GetTermAllTime(ref _sbError);
                return new
                {
                    term_data = count_term_data ,
                    term_all = list
                };
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }
        
    }
}