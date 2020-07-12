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
    public class ExamReportController : BaseUser
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
        public ActionResult<object> GetList(Guid studentId, Guid termId)
        {
            try
            {
                Init();
                var list = _unitOfWorkService.ExamService.GetListByStudentIDAndTermID(studentId, termId, ref _sbError);
                var report = new
                {
                    ExamList = list,
                    ExamAverage = ExamReport.ToOveralAverageExam(list)
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
                if(termId == null)
                {
                    termId = _user.TermID;
                }
                var list = _unitOfWorkReport.ExamReport.ClassSchoolSheetDataTable(_user.SchoolID ,classId, (Guid)termId, ref _sbError);
                return list;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("studentExamHistory")]
        public ActionResult<object> GetStudentSubjectHistory(string regNumber)
        {
            try
            {
                Init();
                var list = _unitOfWorkReport.ExamReport.GetStudentMarkExamHistoryMarksListByRegNumberAndSchoolID(regNumber , _user.SchoolID);
                return list;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("studentSubjectHistory")]
        public ActionResult<object> GetStudentSubjectHistory(string regNumber, string subjectCode)
        {
            try
            {
                Init();
                var list = _unitOfWorkReport.ExamReport.GetStudentMarkExamHistoryMarksListByRegNumberAndSchoolIDAndSubjectCode(regNumber, subjectCode , _user.SchoolID);
                return list;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpGet("levelRanking")]
        public ActionResult<object> GetLevelRanking(Guid levelId, Guid termId, string subjectIds)
        {
            try
            {
                Init();

                List<string> ids = subjectIds.Split(",").ToList();

                List<Guid> sub = new List<Guid>();
                foreach (string subjectId in ids)
                {
                    try
                    {
                        sub.Add(Guid.Parse(subjectId));
                    }
                    catch
                    {

                    }
                }
                var list = _unitOfWorkReport.ExamReport.GetRankListExamByLevelIDAndTermIDAndSubjectIDs(_user.SchoolID , levelId , termId , sub, ref _sbError);
                if (!string.IsNullOrEmpty(_sbError.ToString())){
                    throw new Exception(_sbError.ToString());
                }
                return list;
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

    }
}
