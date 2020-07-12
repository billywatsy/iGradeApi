using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class SubjectService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public SubjectService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public Subject GetSubject(Guid subjectId)
        {
            bool dbFlag = false;
            var list = _uofRepository.SubjectRepository.GetSubject(subjectId, ref dbFlag);
            return list;
        }
        
        public List<Subject> GetSubjects()
        {
            bool dbFlag = false;
            var list = _uofRepository.SubjectRepository.GetListSubjects(_user.SchoolID, ref dbFlag);
            return list;
        }

        public Subject Save(Subject subject , ref StringBuilder sbError)
        {
            bool dbFlag = false;

            if (subject.SubjectID == null || subject.SubjectID == Guid.Empty)
            {
                subject.SchoolID = _user.SchoolID;
            }
            else
            {
                var isLevelFromSchool = _uofRepository.SubjectRepository.GetSubject((Guid)subject.SubjectID, ref dbFlag);

                if (isLevelFromSchool.SubjectID != subject?.SubjectID)
                {
                    sbError.Append("subject not from school");
                    return null;
                }
                subject.SchoolID = isLevelFromSchool.SchoolID;

            }

            var list = _uofRepository.SubjectRepository.GetListSubjects(_user.SchoolID, ref dbFlag);

            if(list.Count() > 100)
            {
                sbError.Append("You have reached maximum subjects allowed");
            }
            else
            {
                if (!string.IsNullOrEmpty(subject.SubjectID.ToString()))
                {
                    var dbSubject = list.Where(c => c.SubjectID == subject.SubjectID).FirstOrDefault();
                    if (dbSubject == null)
                    {
                        sbError.Append("Subject does not exist for school");
                        return null;
                    }
                    else
                    {
                        subject.SubjectCode = dbSubject.SubjectCode;
                        subject.SchoolID = _user.SchoolID;
                    }
                }
                else
                {
                    subject.SchoolID = _user.SchoolID;
                }
            }

            var department = _uofRepository.DepartmentRepository.GetDepartment(subject.DepartmentId, ref dbFlag);

            if(department == null || department.SchoolID != _user.SchoolID)
            {
                sbError.Append("Department does not exist");
                return null;
            }

            var isSaved = _uofRepository.SubjectRepository.Save(subject , _user.Username, ref dbFlag);

            return isSaved;
        }

        public bool Delete(Guid subjectId, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var subject = _uofRepository.SubjectRepository.GetSubject(subjectId, ref dbFlag);

            if (subject == null)
            {
                sbError.Append("subject Does not Exist");
                return false;
            }

            if (subject.SchoolID != _user.SchoolID)
            {
                sbError.Append("Error subject does not exist for school");
                return false;
            }

            return _uofRepository.SubjectRepository.Delete((Guid)subject.SubjectID, _user.Username, ref dbFlag);
        }
    }
}
 