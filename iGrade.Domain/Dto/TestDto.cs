using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class TestDto
    {
        public Guid TestID { get; set; }
        public Guid TeacherClassSubjectID { get; set; }

        [Display(Name = "Test title")]
        public string TestTitle { get; set; }

        [Display(Name = "Out of")]
        public byte OutOf { get; set; }

        [Display(Name = "Date created")]
        public System.DateTime TestDateCreated { get; set; }

        
        public decimal Average { get; set; }

        [Display(Name = "Number passed")]
        public int NumberPassed { get; set; }

        [Display(Name = "Number failed")]
        public int NumberFailed { get; set; }

        [Display(Name = "Total who wrote")]
        public int TotalWritten { get; set; }


        List<TestMarkDto> TestMarkDtos { get; set; }
    }
}
