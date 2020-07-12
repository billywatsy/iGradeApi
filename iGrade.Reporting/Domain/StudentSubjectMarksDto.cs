using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iGrade.Reporting.Domain
{
    public class StudentSubjectMarksDto
    {
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public int SubjectAverage
        {
            get
            {
                if (this.PercentageAscendingByDate == null || this.PercentageAscendingByDate?.Count() <= 0)
                {
                    return 0;
                }
                return Convert.ToInt32(this.PercentageAscendingByDate.Average(c => c.Mark));
            }
        }

        public int TotalTestPassed
        {
            get
            {
                if (this.PercentageAscendingByDate == null || this.PercentageAscendingByDate?.Count() <= 0)
                {
                    return 0;
                }
                return this.PercentageAscendingByDate.Count(c => c.Mark >= 50);
            }
        }

        public int TotalTestFailed
        {
            get
            {
                if (this.PercentageAscendingByDate == null || this.PercentageAscendingByDate?.Count() <= 0)
                {
                    return 0;
                }
                return this.PercentageAscendingByDate.Count(c => c.Mark < 50);
            }
        }
        public int TotalTest
        {
            get
            {
                if (this.PercentageAscendingByDate == null || this.PercentageAscendingByDate?.Count() <= 0)
                {
                    return 0;
                }
                return this.PercentageAscendingByDate.Count();
            }
        }

        public List<StudentSubjectMarksByDateDto> PercentageAscendingByDate { get; set; }
    }

    public class StudentSubjectMarksByDateDto
    {
        public int Mark { get; set; }
        public DateTime Date { get; set; } 
        public int ValueDifferenceFromPreviosMark { get; set; }
    }
}
