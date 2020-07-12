using iGrade.Domain.Dto;
using iGrade.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using iGrade.Reporting.Domain;

namespace iGrade.Reporting.Service
{
   public  class StudentTermReviewReport
    { 
    private UowRepository _uofRepository;
    public StudentTermReviewReport(UowRepository uowRepository)
    {
        _uofRepository = uowRepository;
    } 

    public StudentTermReviewAnasylsisDto GetStudentReview(Guid studentId, Guid termID, ref StringBuilder sbError)
    {
        StudentTermReviewAnasylsisDto studentTermReviewAnasylsisDto = new StudentTermReviewAnasylsisDto();

        studentTermReviewAnasylsisDto.GoodReviews = new List<StudentTermReviewDto>();
        studentTermReviewAnasylsisDto.BadReviews = new List<StudentTermReviewDto>();
        bool dbEror = false;

       var studentEnrollment = _uofRepository.StudentTermRegisterRepository.GetByStudentIDAndTermId(studentId, termID, ref dbEror);

        if(studentEnrollment == null)
        {
            sbError.Append("student not enrollmed for term");
            return studentTermReviewAnasylsisDto;
        }
        var reviews = _uofRepository.StudentTermReviewRepository.GetListByStudentTermRegisterID((Guid)studentEnrollment.StudentTermRegisterID, ref dbEror);
        if (reviews == null)
        {
            return studentTermReviewAnasylsisDto;
        }

        var total = reviews?.Count() ?? 0;
        var totalGood = reviews?.Where(c => c.IsReviewGood == true)?.Count() ?? 0;
        var totalBad = reviews?.Where(c => !c.IsReviewGood)?.Count() ?? 0;

            var totalSumGood = 0;
            var totalSumBad = 0;
            try
            {
                totalSumGood = reviews?.Where(c => c.IsReviewGood == true).Sum(c => c.Star5) ?? 0;
            }
            catch { }
            if (total <= 0) {
                return studentTermReviewAnasylsisDto;
            }

            try
            {
                totalSumBad = reviews?.Where(c => !c.IsReviewGood).Sum(c => c.Star5) ?? 0;
            }
            catch { }

            decimal totalGoodPercentage = 0;

            try
            {
                totalGoodPercentage = ((decimal)totalSumGood / (decimal)(total * 5)) * (decimal)100;
            }
            catch { }
            
            studentTermReviewAnasylsisDto.TotalReviews = total;
            studentTermReviewAnasylsisDto.TotalGoodReviews = totalGood;
            studentTermReviewAnasylsisDto.TotalBadReviews = totalSumBad;
            studentTermReviewAnasylsisDto.OveralGoodStar5Percentage = Convert.ToInt32(totalGoodPercentage);
            studentTermReviewAnasylsisDto.GoodReviews = reviews?.Where(c => c.IsReviewGood == true)?.ToList() ?? new List<StudentTermReviewDto>();
            studentTermReviewAnasylsisDto.BadReviews = reviews?.Where(c => c.IsReviewGood == true)?.ToList() ?? new List<StudentTermReviewDto>();
            return studentTermReviewAnasylsisDto;
    }  
}
}

