using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class SchoolTermYearDataDto
    {
        public Guid ScholID { get; set; }
        public string SchoolName { get; set; }
        public int NumberOfStudents { get; set; }
        public int NumberOfTeachers { get; set; }
        public int TotalNumberOfAbsents { get; set; }
        public int NumberOfClass { get; set; }

        // 20 students     , 40 abstents
        public decimal AverageStudentAbsentRate { get
            {
                if(NumberOfStudents == 0)
                {
                    return 0;
                }
                return Math.Round((decimal)( TotalNumberOfAbsents / NumberOfStudents) , 2 );
            }
        }
    }
}
