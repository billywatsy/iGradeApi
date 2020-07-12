using iGrade.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Reporting.Service
{
    public class UnitOfWorkReport
    {
        private StudentTermRegisterReport studentTermRegisterReportReport;
        private ExamReport examReport;
        private ExamTestReport examTestReport;
        private StudentTermReviewReport studentTermReviewReport;
        private TestReport testReport;
        
        public UnitOfWorkReport(UowRepository uowRepository)
        {
            studentTermRegisterReportReport = new StudentTermRegisterReport(uowRepository);
            examReport = new ExamReport(uowRepository);
            examTestReport = new ExamTestReport(uowRepository);
            studentTermReviewReport = new StudentTermReviewReport(uowRepository);
            testReport = new TestReport(uowRepository);
        }

        public StudentTermRegisterReport StudentTermRegisterReport { get { return studentTermRegisterReportReport; } }
        public ExamReport ExamReport { get { return examReport; } }
        public ExamTestReport ExamTestReport { get { return examTestReport; } }
        public StudentTermReviewReport StudentTermReviewReport { get { return studentTermReviewReport; } }
        public TestReport TestReport { get { return testReport; } }
    }
}
