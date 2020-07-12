using iGrade.Domain;
using iGrade.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Reporting.Domain
{
    public class StudentExamTestMark
    {
        public Student Student { get; set; }
        public List<Reporting.Domain.ExamTest> ExamTestList { get; set; }
        public List<TestMarkDto> TestList { get; set; }
        public Reporting.Domain.StudentTermReviewAnasylsisDto Reviews { get; set; }
        public List<Domain.TestMarkAverage> TestOverallAverage { get; set; }
        public int TestAverage { get; set; }
        public int ExamAverage { get; set; }
    
    }
   
}
