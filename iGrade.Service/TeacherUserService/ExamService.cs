using iGrade.Repository;
using iGrade.Domain;
using iGrade.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class ExamService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public ExamService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user;
        }
         

        public List<ExamDto> GetListExamByTeacherClassSubjectID(Guid teacherClassSubjectID, bool validateIsTeacherForExam, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.ExamRepository.GetListExamByTeacherClassSubjectID(teacherClassSubjectID, _user.SchoolID, ref dbFlag);
            return list;
        }
        
        public List<ExamDto> GetListExamByStudentTermRegisterIDIDAndTermID(Guid studentTermRegisterID, Guid termId , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.ExamRepository.GetListExamByStudentTermRegisterIDAndTermID(studentTermRegisterID, termId, ref dbFlag);
            return list;
        }

        public List<ExamDto> GetListExamByStudentID(Guid studentID, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.ExamRepository.GetListByStudentID(studentID, ref dbFlag);
            return list;
        }


        public List<ExamDto> GetListByStudentIDAndTermID(Guid studentID, Guid termID , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.ExamRepository.GetListByStudentIDAndTermID(studentID, termID, ref dbFlag);
            return list;
        }
        public List<ExamDto> GetDeletedListExamByTeacherClassSubjectID(Guid teacherClassSubjectID, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.ExamRepository.GetDeletedListExamByTeacherClassSubjectID(teacherClassSubjectID  , _user.SchoolID, ref dbFlag);
            return list;
        }
        
        public List<ExamDto> GetListExamByStudentIDAndSubjectCode(Guid studentId, string subjectCode, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.ExamRepository.GetListExamByStudentIdAndSubjectCode(_user.SchoolID, studentId, subjectCode, ref dbFlag);
            if (list == null)
            {
                list = new List<ExamDto>();
            }
            return list;
        }

        public List<ExamDto> GetListExamByClassIDAndTermID(Guid ClassID, Guid TermID, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.ExamRepository.GetListByClassIDandTermID(_user.SchoolID, ClassID, TermID, ref dbFlag);
            if (list == null)
            {
                list = new List<ExamDto>();
            }
            return list;
        }

        public List<ExamDto> GetListByLevelIDandTermID(Guid LevelID, Guid TermID, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.ExamRepository.GetListByLevelIDandTermID(_user.SchoolID, LevelID, TermID, ref dbFlag);
            if (list == null)
            {
                list = new List<ExamDto>();
            }
            return list;
        }


        public int SaveBulkByRegNumber(List<iGrade.Domain.Form.ExamSaveFORM> exam, Guid teacherClassSubjectID, ref List<string> lsError, ref List<string> lsSuccess)
        {
            if(exam == null)
            {
                lsError.Add("no data found in request");
                return 0;
            }
            bool dbFlag = false;
            var teacherClassSubject = _uofRepository.TeacherClassSubjectRepository.GetTeacherClassSubjectByID(teacherClassSubjectID, ref dbFlag);
            if (teacherClassSubject == null)
            {
                lsError.Add("teacher class subject does not exist");
                return 0;
            }
            var enrollment = _uofRepository.StudentTermRegisterRepository.GetListByClassIDAndTermID(teacherClassSubject.ClassID, teacherClassSubject.TermID, ref dbFlag);

            var newListForClass = new List<Exam>();
            foreach (var studentExam in exam)
            {
                var isStudentExist = enrollment.Where(c => c.RegNumber?.ToLower() == studentExam?.RegNumber?.ToLower()).FirstOrDefault();
                if(isStudentExist == null)
                {
                    lsError.Add($"{studentExam.RegNumber} is not registered in class ");
                    continue;
                }
                newListForClass.Add(new Exam()
                    {
                        StudentTermRegisterID = isStudentExist.StudentTermRegisterID ,
                        TeacherClassSubjectID = teacherClassSubjectID ,
                        Mark = studentExam.Mark ,
                        Comment = studentExam.Comment
                    }
                );
            }
            return Save(newListForClass , teacherClassSubjectID , ref lsError , ref lsSuccess);
        }

        /// <summary>
        /// number of records update/insert
        /// </summary>
        /// <param name="exam"></param>
        /// <param name="teacherClassSubjectID"></param>
        /// <param name="lsError"></param>
        /// <param name="lsSuccess"></param>
        /// <returns></returns>
        public int Save(List<Exam> exam, Guid teacherClassSubjectID , ref List<string> lsError , ref List<string> lsSuccess)
        {
            // check if admin has not locked exam submission 
            
            // 1:  First Check exam
            // 3: check if class and student belong to same school 
            // 4: Check if class and teacher belongto same school and is teacher
            // 2: Check if enrolled in class
            // 5: check mark is withing range
            // 6: grade mark 
            // 7: filter comment
            // 8: save exam
            bool dbFlag = false;

            var setting = _uofRepository.SettingRepository.GetSettingBySchoolID(_user.SchoolID, ref dbFlag);

            StringBuilder sbError = new StringBuilder("");
            var isClosed = IsExamSubmissionClosed(ref sbError);

            if (isClosed)
            {
                lsError.Add(sbError.ToString());
                return 0;
            }

            var subjectClassDetails = _uofRepository.TeacherClassSubjectRepository.GetTeacherClassSubjectByID(teacherClassSubjectID, ref dbFlag);
            if (subjectClassDetails == null) 
            {
                lsError.Add("Failed getting Teacher class Subject details for exam");
                return 0;
            }

            if (subjectClassDetails.TeacherID != _user.TeacherID )
            {
                if (!_user.isAdmin)
                {
                    lsError.Add("You dont have priviledge save exams");
                    return 0;
                }
            }
            
            if (subjectClassDetails.SchoolID != _user.SchoolID) 
            {
                lsError.Add("Class does not belong to school ");
                return 0;
            }

            var studentInClassByEnrollment = _uofRepository.StudentTermRegisterRepository.GetListByClassIDAndTermID(subjectClassDetails.ClassID, _user.TermID , ref dbFlag);
            if (studentInClassByEnrollment == null)
            {
                lsError.Add("No student enrolled in that class ");
                return 0;
            }

            bool gradeError = false;
            var grade = _uofRepository.GradeRepository.GetGradeById(subjectClassDetails.GradeID, ref dbFlag);
            if(grade == null)
            {
                lsError.Add("grade does not exist");
                return 0;
            }

            var gradeList = _uofRepository.GradeMarkRepository.GetGradeMarkListByGradeId(subjectClassDetails.GradeID, ref dbFlag) ?? new List<GradeMark>();

            List<Exam> filteredExam = new List<Exam>();
            foreach(var exa in exam)
            {
                var isStudentExistInClass = studentInClassByEnrollment.Where(c => c.StudentTermRegisterID == exa.StudentTermRegisterID).FirstOrDefault();
                if (isStudentExistInClass == null)
                {
                    lsError.Add("student not enrolled in  class ");
                    continue;
                }
                else
                {
                    exa.StudentTermRegisterID = (Guid)isStudentExistInClass.StudentTermRegisterID;
                }
                
                if (exa.Mark < 0)
                {
                    lsError.Add("Mark should not be greater than zero ");
                    continue;
                }
                else if (exa.Mark > 100)
                {
                    lsError.Add("Mark should not be less than 100 ");
                    continue;
                }
                bool errorGrade = false;
                var defaultComment = "No Comment";

                
                var gradeValue = iGrade.Core.TeacherUserService.Common.GradeCalculateCommon.Grade(grade , gradeList , Convert.ToInt32(exa.Mark), ref defaultComment , ref errorGrade);

                if (string.IsNullOrEmpty(exa.Comment))
                {
                    exa.Comment = defaultComment;
                }

                if (errorGrade)
                {
                    lsError.Add("No Grade for within that range for grade type :" + grade.Description);
                    continue;
                }
                filteredExam.Add(new Exam() {
                    StudentTermRegisterID = exa.StudentTermRegisterID,
                                              Mark =  exa.Mark ,
                                              Comment = exa.Comment  , 
                                              TeacherClassSubjectID = teacherClassSubjectID ,
                                              Grade = gradeValue
                });
            }

            if(filteredExam == null) return 0;

            var numberSaved = _uofRepository.ExamRepository.Save(filteredExam, _user.Username , ref dbFlag);

             lsSuccess.Add($"{numberSaved} recors(s) saved succcessfully");

            _uofRepository.LogRepository.Save(new Log() { TeacherID = _user.TeacherID, EntityType = "Exam", ActionDetails = $"{numberSaved} records saved", ActionType = "Saved" }, ref dbFlag);
            return numberSaved;
        }

        public bool IsExamSubmissionClosed(ref StringBuilder sbError)
        {
            var dbFlag = false;
            var setting = _uofRepository.SettingRepository.GetSettingBySchoolID(_user.SchoolID, ref dbFlag);

            if (setting == null)
            {
                sbError.Append("Failed getting school setting");
                return true;
            }

            if (setting.ExamMarkSubmissionClosingDate < DateTime.UtcNow)
            {
                sbError.Append("Date has been closed for subim getting school setting");
                return true;
            }
            return false;
        }
        public bool Delete(List<Guid> examList, Guid TeacherClassSubjectID , ref int numberOfRecordsDeleted , ref StringBuilder sbError)
        {
            numberOfRecordsDeleted = 0;
            if (examList == null || examList.Count() <= 0)
            {
                sbError.Append("Not data found");
                return false;
            }

            bool dbFlag = false;

            var isClosed = IsExamSubmissionClosed(ref sbError);

            if (isClosed)
            { 
                return false;
            }
            // check submission deadline has been closed
            var isResponsibleForSubject = _uofRepository.TeacherClassSubjectRepository.GetTeacherClassSubjectByID(TeacherClassSubjectID, ref dbFlag);

            var studentEnrollment = _uofRepository.ExamRepository.GetListExamByTeacherClassSubjectID(TeacherClassSubjectID , _user.SchoolID , ref dbFlag);
            if (isResponsibleForSubject == null)
            {
                sbError.Append("Record for teacher class subject does not exist or has been deleted");
                return false;
            }
            else if (studentEnrollment == null)
            {
                sbError.Append("Student mark does not exist or it has been deleted , check deleted records");
                return false;
            }
            else
            {
                if(isResponsibleForSubject.TeacherID != _user.TeacherID)
                {
                    sbError.Append("You dont teacher that student");
                    return false;
                }
            }

            foreach (var exam in examList)
            {
                if (studentEnrollment.Where(c => c.StudentTermRegisterID == exam).FirstOrDefault() != null)
                {
                    var isDeleted = false;
                    isDeleted = ExamDelete(exam, TeacherClassSubjectID, ref dbFlag);
                    if (isDeleted)
                    {
                        numberOfRecordsDeleted++;
                    }
                }
            }
            _uofRepository.LogRepository.Save(new Log() { TeacherID = _user.TeacherID, EntityType = "Exam", ActionDetails = $"{numberOfRecordsDeleted} records deleted for class name" + isResponsibleForSubject?.ClassName, ActionType = "Deleted" }, ref dbFlag);
            return true;
        }

        private bool ExamDelete(Guid EnrollmentID , Guid TeacherClassSubjectID , ref bool dbFlag)
        {
            var isDeleted = _uofRepository.ExamRepository.Delete(EnrollmentID, TeacherClassSubjectID , _user.Username , ref dbFlag);
            return isDeleted;
        } 
        public List<ExamDto> ExcelSession(List<ExamDto> exam, Guid teacherClassSubjectID, ref List<string> lsError )
        {
            bool dbFlag = false;

            var setting = _uofRepository.SettingRepository.GetSettingBySchoolID(_user.SchoolID, ref dbFlag);
            if (setting == null)
            {
                lsError.Add("Failed getting school settings for exam");
                return null;
            }


            var subjectClassDetails = _uofRepository.TeacherClassSubjectRepository.GetTeacherClassSubjectByID(teacherClassSubjectID, ref dbFlag);
            if (subjectClassDetails == null)
            {
                lsError.Add("Failed getting Teacher class Subject details for exam");
                return null;
            }

            if (subjectClassDetails.TeacherID != _user.TeacherID)
            {
                if (!_user.isAdmin)
                {
                    lsError.Add("You dont have priviledge save exams");
                    return null;
                }
            }

            if (subjectClassDetails.SchoolID != _user.SchoolID)
            {
                lsError.Add("Class does not belong to school ");
                return null;
            }

            var studentInClassByEnrollment = _uofRepository.StudentTermRegisterRepository.GetListByClassIDAndTermID(subjectClassDetails.ClassID, _user.TermID, ref dbFlag);
            if (studentInClassByEnrollment == null)
            {
                lsError.Add("No student enrolled in that class ");
                return null;
            }
            List<ExamDto> filteredExam = new List<ExamDto>();
            foreach (var exa in exam)
            {
                var isStudentExistInClass = studentInClassByEnrollment.Where(c => c.RegNumber == exa.RegNumber).FirstOrDefault();
                if (isStudentExistInClass == null)
                {
                    lsError.Add("student not enrolled in  class ");
                    continue;
                }
                else
                {
                    exa.StudentTermRegisterID = (Guid)isStudentExistInClass.StudentTermRegisterID;
                }

                if (exa.Mark < 0)
                {
                    lsError.Add("Mark should not be greater than zero ");
                    continue;
                }
                else if (exa.Mark > 100)
                {
                    lsError.Add("Mark should not be less than 100 ");
                    continue;
                }
                var grade = _uofRepository.GradeRepository.GetGradeById(subjectClassDetails.GradeID, ref dbFlag);
                if (grade == null)
                {
                    lsError.Add("grade does not exist");
                    return null;
                }

                var gradeList = _uofRepository.GradeMarkRepository.GetGradeMarkListByGradeId(subjectClassDetails.GradeID, ref dbFlag) ?? new List<GradeMark>();

                
                var defaultComment = "No Comment";

                bool errorGrade = false;

                var gradeValue = iGrade.Core.TeacherUserService.Common.GradeCalculateCommon.Grade(grade, gradeList, Convert.ToInt32(exa.Mark), ref defaultComment, ref errorGrade);

                if (string.IsNullOrEmpty(exa.Comment))
                {
                    exa.Comment = defaultComment;
                }

                filteredExam.Add(new ExamDto()
                {
                    StudentTermRegisterID = exa.StudentTermRegisterID, 
                    RegNumber = exa.RegNumber , 
                    Mark = exa.Mark,
                    Comment = exa.Comment,
                    TeacherClassSubjectID = teacherClassSubjectID,
                    Grade = gradeValue
                });
            }
            return filteredExam;
        }
      
    }
}
 