using iGrade.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Reporting.Domain
{
    public class StudentTermReviewAnasylsisDto
    {
        public int TotalReviews { get; set; }
        public int TotalGoodReviews { get; set; }
        public int TotalBadReviews { get; set; }
        public int OveralGoodStar5Percentage { get; set; }
        public List<StudentTermReviewDto> GoodReviews{ get; set; }
        public List<StudentTermReviewDto> BadReviews{ get; set; }
    }
}
