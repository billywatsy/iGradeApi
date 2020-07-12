using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Reporting.Domain
{
    public class StudentTermMarkAverage
    {
        public DateTime TermFromDate { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public int Average { get; set; }
    }
}
