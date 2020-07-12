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
    public class ExamTestReportController : BaseUser
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
        
        [HttpGet("studentTerm")]
        public ActionResult<object> GetList(Guid? studentId  , Guid? termId)
        {
            try
            {
                Init();
                List<TestMarkDto> allTest = new List<TestMarkDto>();
                List<ExamDto> allExam = new List<ExamDto>();
                var report = _unitOfWorkReport.ExamTestReport.StudentReportByStudentAndTerm((Guid)studentId, (Guid)termId, ref allExam, ref allTest, ref _sbError) ?? new List<Reporting.Domain.ExamTest>();
                
                return report;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }
    }
}
