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
    public class LessonPlanService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public LessonPlanService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public LessonPlanDto GetLessonPlan(Guid lessonPlanId, ref StringBuilder sbError)
        { 
            bool dbFlag = false;
            var data = _uofRepository.LessonPlanRepository.GetByLessonPlanID(lessonPlanId, ref dbFlag);
            return data;
        }
        public List<LessonPlanDto> GetListLessonPlansByTeacherClassSubjectID(Guid teacherClassSubjectID, ref StringBuilder sbError)
        { 
            bool dbFlag = false;
            var list = _uofRepository.LessonPlanRepository.GetListLessonPlansByTeacherClassSubjectID(teacherClassSubjectID, ref dbFlag);
            return list;
        }

        public Guid Save(LessonPlan lessonPlan, ref StringBuilder sbError)
        {

            if (lessonPlan == null)
            {
                sbError.Append("Fill in all required fields");
                return Guid.Empty;
            }


            bool dbFlag = false;

            var teacherClassSubject = _uofRepository.TeacherClassSubjectRepository.GetTeacherClassSubjectByID(lessonPlan.TeacherClassSubjectId, ref dbFlag);


            if (teacherClassSubject == null)
            {
                sbError.Append("Teacher class subject does not exist");
                return Guid.Empty;
            }

            if (teacherClassSubject.TeacherID != _user.TeacherID)
            {
                sbError.Append("You are not th teacher for the subject ");
                return Guid.Empty;
            }

            if (string.IsNullOrEmpty(lessonPlan.Body))
            {
                sbError.Append("Lesson Plan body is required ");
                return Guid.Empty;
            }
            

            if (lessonPlan.LessonPlanId == null || lessonPlan.LessonPlanId == Guid.Empty)
            {
                return Insert(lessonPlan , ref sbError);
            }


            return Guid.Empty;
        }

        private Guid Insert(LessonPlan lessonPlan, ref StringBuilder sbError)
        {
            // check if lesson plan exist for that date range
            bool dbFlag = false;
            var allLessonPlans = _uofRepository.LessonPlanRepository.GetListLessonPlansByTeacherClassSubjectID(lessonPlan.TeacherClassSubjectId, ref dbFlag);

            if (allLessonPlans == null)
            {

                var isRangeExist = allLessonPlans.Where(c => c.DateStart >= lessonPlan.DateStart && c.DateEnd <= lessonPlan.DateEnd).FirstOrDefault();

                if (isRangeExist != null)
                {
                    sbError.Append("There is a lesson plan for that date already");
                    return Guid.Empty;
                }
            }
            var save = _uofRepository.LessonPlanRepository.Save(lessonPlan , _user.Username , ref dbFlag);
            if(save == null)
            {

                sbError.Append("Error saving lesson plan");
                return Guid.Empty;
            }
            return (Guid)save.LessonPlanId;
        }

        private Guid Update(LessonPlan lessonPlan, ref StringBuilder sbError)
        {
            bool dbFlag = false;

            var lessonPlanObj = _uofRepository.LessonPlanRepository.GetByLessonPlanID((Guid)lessonPlan.LessonPlanId, ref dbFlag);
            if (lessonPlanObj != null)
            {
                sbError.Append("Lesson Pla does not exist");
                return Guid.Empty;
            }

            // check if lesson plan exist for that date range

            var allLessonPlans = _uofRepository.LessonPlanRepository.GetListLessonPlansByTeacherClassSubjectID(lessonPlan.TeacherClassSubjectId, ref dbFlag);

            if (allLessonPlans == null)
            {

                var isRangeExist = allLessonPlans.Where(c => c.DateStart >= lessonPlan.DateStart && c.DateEnd <= lessonPlan.DateEnd && c.LessonPlanId != (Guid)lessonPlan.LessonPlanId).FirstOrDefault();

                if (isRangeExist != null)
                {
                    sbError.Append("There is a lesson plan for that date already");
                    return Guid.Empty;
                }
            }

            var setting = _uofRepository.SettingRepository.GetSettingBySchoolID(_user.SchoolID, ref dbFlag);
            if (setting != null)
            {
                sbError.Append("Failed getting setting");
                return Guid.Empty;
            }

            if (DateTime.Now.Subtract(lessonPlan.CreatedDate).Days > setting.LessonPlanNotEditedAfterDaysOfCreation)
            {
                sbError.Append("Record can has been locked for updates/edits ");
                return Guid.Empty;
            }

            if (lessonPlan.ApprovedByID != null)
            {
                sbError.Append("Record has been approved and can no longer be edited ");
                return Guid.Empty;
            }

            var save = _uofRepository.LessonPlanRepository.Save(lessonPlan, _user.Username, ref dbFlag);
            if (save == null)
            {
                sbError.Append("Error updating lesson plan");
                return Guid.Empty;
            }
            return (Guid)save.LessonPlanId;
        }



        public LessonPlanDto ApproveLessonPlan(Guid lessonPlanId, ref StringBuilder sbError)
        {
            bool dbFlag = false;

            var lessonPlanObj = _uofRepository.LessonPlanRepository.GetByLessonPlanID(lessonPlanId, ref dbFlag);
            if (lessonPlanObj == null)
            {
                sbError.Append("Lesson Plan does not exist");
                return null;
            }

            var teacherClassSubject = _uofRepository.TeacherClassSubjectRepository.GetTeacherClassSubjectByID(lessonPlanObj.TeacherClassSubjectId, ref dbFlag);


            if (teacherClassSubject == null)
            {
                sbError.Append("Teacher class subject does not exist");
                return null;
            }

            // check if allowed to monitor here

            var setting = _uofRepository.SettingRepository.GetSettingBySchoolID(_user.SchoolID, ref dbFlag);
            if (setting == null)
            {
                sbError.Append("Failed getting setting");
                return null;
            }

            

            if (lessonPlanObj.ApprovedByID != null)
            {
                sbError.Append("Record has already been approved and can no longer be edited ");
                return null;
            }
            lessonPlanObj.ApprovedByID = _user.TeacherID;
            var save = _uofRepository.LessonPlanRepository.SaveApproval((Guid)lessonPlanObj.LessonPlanId,_user.TeacherID , _user.Username, ref dbFlag);
            if (!save )
            {
                sbError.Append("Error approving lesson plan");
                return null;
            }
            lessonPlanObj.ApprovedByID = _user.TeacherID;
            lessonPlanObj.ApprovedByName = _user.TeacherFullname;
            
            return lessonPlanObj;
        }


        public bool Delete(Guid lessonPlanId ,  ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var setting = _uofRepository.SettingRepository.GetSettingBySchoolID(_user.SchoolID, ref dbFlag);
            if (setting == null)
            {
                sbError.Append("Failed getting setting");
                return false;
            }

            var lessonPlan = _uofRepository.LessonPlanRepository.GetByLessonPlanID(lessonPlanId, ref dbFlag);
            if (lessonPlan == null)
            {
                sbError.Append("Lesson Plan does not exist");
                return false;
            }
            if (DateTime.Now.Subtract(lessonPlan.CreatedDate).Days > setting.LessonPlanNotEditedAfterDaysOfCreation)
            {
                sbError.Append("Record can has been locked for updates/edits ");
                return false;
            }

            if (lessonPlan.ApprovedByID != null)
            {
                sbError.Append("Record has been approved and can no longer be edited ");
                return false;
            }
            
           return _uofRepository.LessonPlanRepository.Delete(lessonPlanId, _user.Username, ref dbFlag);
        }
    }
}
 