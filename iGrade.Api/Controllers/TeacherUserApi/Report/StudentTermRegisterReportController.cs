using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iGrade.Core.TeacherUserService;
using iGrade.Domain.Dto;
using iGrade.Reporting.Service;
using iGrade.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGrade.Api.Controllers.TeacherUserApi.Report
{
    [Route("api/s/[controller]")]
    [ApiController]
    public class StudentTermRegisterReportController : BaseUser
    {
        UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;
        private UnitOfWorkReport _unitOfWorkReport;
        private UnitOfWorkService _unitOfWorkService;
        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _sbError = new StringBuilder("");
            _unitOfWorkReport = new UnitOfWorkReport(_repository);
            _unitOfWorkService = new UnitOfWorkService(_user, _repository);
        }
        
        [HttpGet("currentTerm")]
        public ActionResult<object> GetList(Guid? termId)
        {
            try
            {
                Init();
                List<string> lts = new List<string>();
                if(termId == null)
                    termId =_unitOfWorkService.SettingService.GetSchoolSetting(_user.SchoolID, ref lts)?.TermID;
                return _unitOfWorkReport.StudentTermRegisterReport.GetTermStudentTermRegister(_user.SchoolID , (Guid)termId, ref _sbError);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }
 
    }
}
