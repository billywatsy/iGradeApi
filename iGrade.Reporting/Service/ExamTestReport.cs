using iGrade.Domain.Dto;
using iGrade.Reporting.Domain;
using iGrade.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace iGrade.Reporting.Service
{
    public class ExamTestReport
    {
        private UowRepository _uofRepository; 
        public ExamTestReport(UowRepository uowRepository)
        {
            _uofRepository = uowRepository; 
        }
         
        public List<ExamTest> StudentReportByStudentAndTerm(Guid studentId, Guid termID,ref List<ExamDto> exams , ref List<TestMarkDto> allTests ,  ref StringBuilder sbError)
        {
            bool dbEror = false;
            var student = _uofRepository.StudentTermRegisterRepository.GetByStudentIDAndTermId(studentId, termID, ref dbEror);
            if (student == null)
            {
                sbError.Append("Student was not registered for that term");
                return null;
            }
            var studentInformation = _uofRepository.StudentTermRegisterRepository.GetByID((Guid)student.StudentTermRegisterID, ref dbEror);
            var term = _uofRepository.TermRepository.GetByID(termID, ref dbEror);

            if (studentInformation == null)
            {
                sbError.Append("Student was not registered for that term");
                return null;
            }

            if (term == null)
            {
                sbError.Append("Term does not exist");
                return null;
            }
            var studentExam = _uofRepository.ExamRepository.GetListExamByStudentTermRegisterIDAndTermID((Guid)student.StudentTermRegisterID, termID, ref dbEror) ?? new List<ExamDto>();
            var studentTest = _uofRepository.TestMarkRepository.GetListTestMarksByStudentTermRegisterIDAndTermID((Guid)student.StudentTermRegisterID, termID, ref dbEror) ?? new List<TestMarkDto>();
            allTests = studentTest;
            exams = studentExam;
            List<ExamTest> report = new List<ExamTest>();
            foreach (var mark in studentExam)
            {
                var testList = studentTest.Where(c => c.SubjectCode == mark.SubjectCode);

                ExamTest row = new ExamTest();
                row.SubjectCode= mark.SubjectCode;
                row.SubjectName = mark.SubjectName;
                row.Grade = mark.Grade;
                row.Exam = mark.Mark;
                row.TestWritten = testList?.Count() ?? 0;
                
                var testAverageValue = 0;

                
                if (row.TestWritten > 0)
                {
                    testAverageValue = Convert.ToInt32(testList.Average(c => c.MarkPercentage));
                }

                row.TestAverage = testAverageValue;
                
                try
                {
                    row.TestPassed =  testList?.Where(c => c.MarkPercentage >= 50M)?.Count() ?? 0;
                }
                catch
                {
                }

                try
                {
                    row.TestFailed = testList?.Where(c => c.MarkPercentage < 50M)?.Count() ?? 0;
                }
                catch
                {

                }
                var examAverage = mark.Mark;
                var variancePercentage = mark.Mark - testAverageValue; 
                if (variancePercentage <= 0)
                {
                    variancePercentage = -1 * variancePercentage;
                }
                row.VariancePercentage = variancePercentage;
                report.Add(row);
            }

            if(studentTest == null || studentTest.Count() <= 0)
            {
                return report;
            }

            var uniqueSubject = studentTest.Select(c => c.SubjectCode).Distinct();

            foreach (var subject in uniqueSubject)
            {
                var isExistInExam = report?.Where(c => c.SubjectCode == subject)?.FirstOrDefault();

                if(isExistInExam != null)
                {
                    continue;
                }
                var testList = studentTest?.Where(c => c.SubjectCode == subject);
                var mark = testList?.FirstOrDefault();

                if(mark == null)
                {
                    continue;
                }
                ExamTest row = new ExamTest();
                row.SubjectCode = mark.SubjectCode;
                row.SubjectName = mark.SubjectName;
                row.Grade = "";
                row.Exam = 0;
                row.TestWritten = testList?.Count() ?? 0;
                 

                if (row.TestWritten > 0)
                {
                    row.TestAverage = Convert.ToInt32(testList.Average(c => c.MarkPercentage));
                }
                 

                try
                {
                    row.TestPassed = testList?.Where(c => c.MarkPercentage >= 50M)?.Count() ?? 0;
                }
                catch
                {
                }

                try
                {
                    row.TestFailed = testList?.Where(c => c.MarkPercentage < 50M)?.Count() ?? 0;
                }
                catch
                {

                }
                 
                row.VariancePercentage = row.TestAverage;

                report.Add(row);

            }


            return report;
        }

    }
}
