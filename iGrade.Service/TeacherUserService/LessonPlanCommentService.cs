using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iGrade.Domain.Dto;

namespace iGrade.Core.TeacherUserService
{
    public class LessonPlanCommentService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public LessonPlanCommentService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }
        
        public List<LessonPlanCommentDto> GetListByLessonPlanID(Guid lessonPlanID, ref StringBuilder sbError)
        { 
            bool dbFlag = false;
            var list = _uofRepository.LessonPlanCommentRepository.GetListByLessonPlanID(lessonPlanID, _user.TeacherID , ref dbFlag);
            return list;
        }

        /// <summary>
        /// Only a teacher can save his on staff
        /// </summary>
        /// <param name="lessonPlanComment"></param>
        /// <param name="sbError"></param>
        /// <returns></returns>
        public LessonPlanCommentDto Save(LessonPlanComment lessonPlanComment, ref StringBuilder sbError)
        {

            if (lessonPlanComment == null)
            {
                sbError.Append("Fill in all required fields");
                return null;
            }

            bool dbFlag = false;

            var lessonPlan = _uofRepository.LessonPlanRepository.GetByLessonPlanID(lessonPlanComment.LessonPlanId, ref dbFlag);
            
            if (lessonPlan == null)
            {
                sbError.Append("Lesson Plan does not exist");
                return null;
            }
            
            if (string.IsNullOrEmpty(lessonPlanComment.Comment))
            {
                sbError.Append("comment is required");
                return null;
            }

            if(lessonPlanComment.LessonPlanCommentId != null && lessonPlanComment.LessonPlanCommentId != Guid.Empty)
            {

                var comment = _uofRepository.LessonPlanCommentRepository.GetByLessonPlanCommentID((Guid)lessonPlanComment.LessonPlanCommentId, ref dbFlag);

                if (comment == null)
                {
                    sbError.Append("lesson plan comment does not exist required");
                    return null;
                }
                else
                {
                    if(comment.TeacherId != _user.TeacherID)
                    {
                        sbError.Append("lesson plan comment does not belong to you");
                        return null;
                    }
                }
            }
            else
            {
                lessonPlanComment.TeacherId = _user.TeacherID;
            }
            
            var save = _uofRepository.LessonPlanCommentRepository.Save(lessonPlanComment, _user.Username, ref dbFlag);

            var lessonComment = _uofRepository.LessonPlanCommentRepository.GetListByLessonPlanID(save.LessonPlanId, _user.TeacherID, ref dbFlag)?.FirstOrDefault(c => c.LessonPlanCommentId ==  save.LessonPlanCommentId);


            return lessonComment;
        }
        
        public bool Delete(Guid lessonPlanCommentId ,  ref StringBuilder sbError)
        {
            bool dbFlag = false;
            
           return _uofRepository.LessonPlanCommentRepository.Delete(lessonPlanCommentId, _user.Username, ref dbFlag);
        }
    }
}
 