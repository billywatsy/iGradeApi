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
    public class TestMarkReportController : BaseUser
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
        public ActionResult<object> GetList(Guid studentId  , Guid termId)
        {
            try
            {
                Init();
                var list = _unitOfWorkService.TestMarkService.GetListTestMarksByStudentIDAndTermID(studentId, termId, ref _sbError);
                var report = new
                {
                    TestMarkList = list,
                    AverageList = TestReport.ToAverageTestMark(list),
                    Average = TestReport.ToOveralAverageTestMark(list)
                };
                return report;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("classSheet")]
        public ActionResult<object> GetClassExamSheet(Guid classId, Guid? termId)
        {
            try
            {
                Init();
                if (termId == null)
                {
                    termId = _user.TermID;
                }
                var list = _unitOfWorkReport.TestReport.GetClassTestAverages(classId, (Guid)termId);
                return list;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("studentSubjectHistory")]
        public ActionResult<object> GetStudentSubjectHistory(Guid studentTermRegisterId, string subjectCode)
        {
            try
            {
                Init(); 
                var list = _unitOfWorkReport.TestReport.GetTestMarkBottomStudentSubjectHistory(studentTermRegisterId, subjectCode);
                return list;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }
    }
}
