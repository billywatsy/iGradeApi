using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Reporting.Domain
{
    public class TestMarkAverage
    {
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; } 
        public int TestWritten { get; set; }
        public int TestAverage { get; set; }
        public int TestPassed { get; set; }
        public int TestFailed { get; set; } 
    }
}
