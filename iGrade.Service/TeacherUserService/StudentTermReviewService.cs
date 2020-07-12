using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class StudentTermReviewService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public StudentTermReviewService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public List<iGrade.Domain.Dto.StudentTermReviewDto> GetListByTeacherReviewsByYearAndMonth(Guid teacherid , DateTime date)
        {
            bool dbFlag = false;
            return _uofRepository.StudentTermReviewRepository.GetListByTeacherReviewsByYearAndMonth(teacherid, date , ref dbFlag);
        }

        public List<iGrade.Domain.Dto.StudentTermReviewDto> GetListByStudentTermRegisterID(Guid studentTermRegisterID)
        {
            bool dbFlag = false;
            return _uofRepository.StudentTermReviewRepository.GetListByStudentTermRegisterID(studentTermRegisterID, ref dbFlag);
        }
        
        public bool Delete(Guid studentreviewid)
        {
            bool dbFlag = false;
            return _uofRepository.StudentTermReviewRepository.Delete(studentreviewid, _user.Username, ref dbFlag);
        }


        public bool  Save(Domain.Form.StudentTermReviewForm studentReview , ref StringBuilder sbError)
        {

            bool dbFlag = false;
            if (studentReview == null)
            {
                sbError.Append("Fill in all fields");
                return false;
            }

            var student = _uofRepository.StudentRepository.GetStudentByRegNumber(studentReview.RegNumber, _user.SchoolID, ref dbFlag);
            if (studentReview == null)
            {
                sbError.Append($"student reg number [{studentReview.RegNumber}] does not exist");
                return false;
            }

            var enrollment = _uofRepository.StudentTermRegisterRepository.GetByStudentIDAndTermId((Guid)student.StudentID, _user.TermID, ref dbFlag);
            if (enrollment == null)
            {
                sbError.Append($"student  reg number [{studentReview.RegNumber}] can only be reviewed when he is enrolled");
                return false;
            }

            var studentEnrollment = _uofRepository.StudentTermRegisterRepository.GetByID((Guid)enrollment.StudentTermRegisterID, ref dbFlag);

            if(studentEnrollment == null)
            {
                sbError.Append("student was not registered for this term");
                return false;
            }

            var studentObj = _uofRepository.StudentRepository.GetStudentById(studentEnrollment.StudentID, ref dbFlag);

            if (studentObj == null)
            {
                return false;
            }
            
            if(studentObj.SchoolID != _user.SchoolID)
            {
                sbError.Append("student does not belong to school");
                return false;
            }
            var teacherReviews = _uofRepository.StudentTermReviewRepository.GetListByTeacherReviewsByYearAndMonth(_user.TeacherID , DateTime.Now , ref dbFlag);

            if(teacherReviews != null)
            {
                var dailyTermLimit = teacherReviews.Count();
                if(dailyTermLimit >= 500)
                {
                    sbError.Append("Teacher has reached limit");
                }
            }

            var studentListReviews = _uofRepository.StudentTermReviewRepository.GetListByStudentTermRegisterID((Guid)enrollment.StudentTermRegisterID, ref dbFlag);

            if(studentListReviews != null)
            {
                var hasTeacherSubmittedForToday = studentListReviews.Where(c => c.TeacherID == _user.TeacherID)
                                                                    .Where(c => c.CreatedDate.Year == DateTime.Now.Year)
                                                                    .Where(c => c.CreatedDate.Month == DateTime.Now.Month)
                                                                    .Where(c => c.CreatedDate.Day == DateTime.Now.Day)
                                                                    .Count();

                if(hasTeacherSubmittedForToday >= 2)
                {
                    sbError.Append("you can not add more than two reviews for a day for the particular student");
                    return false;
                }
            }
            if (studentReview.Star5 < 1 || studentReview.Star5 > 5) {
                sbError.Append("review star is in bad format");
                return false;
            }

                return _uofRepository.StudentTermReviewRepository.Save(new StudentTermReview() {
                        StudentTermRegisterID = (Guid)enrollment.StudentTermRegisterID ,
                        TeacherID = _user.TeacherID , 
                        IsReviewGood = studentReview.IsReviewGood , 
                        Body  = studentReview.Body , 
                        CreatedDate = DateTime.Now ,
                        Star5 = studentReview.Star5 
                         } , _user.Username, ref dbFlag);
        }

    }
}
 