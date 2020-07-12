using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Reporting.Domain
{
    public class ExamTest
    {
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string Grade { get; set; }
        public int Exam { get; set; }
        public int TestWritten { get; set; }
        public int TestAverage { get; set; }
        public int TestPassed { get; set; }
        public int TestFailed { get; set; }
        public int VariancePercentage { get; set; }
    }
}
