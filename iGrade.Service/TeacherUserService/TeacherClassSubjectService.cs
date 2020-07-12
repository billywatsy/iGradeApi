using iGrade.Repository;
using iGrade.Domain;
using iGrade.Domain.Dto;
using iGrade.Domain.Form; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class TeacherClassSubjectService
    {
        private UowRepository _uofRepository;
        private Domain.Dto.LoggedUser _user;
        public TeacherClassSubjectService(Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user;
        }
        public TeacherClassSubjectDto GetTeacherClassSubjectsByID(Guid teacherClassSubjectId, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var classSubjectList = _uofRepository.TeacherClassSubjectRepository.GetTeacherClassSubjectByID(teacherClassSubjectId, ref dbFlag);

            if (classSubjectList == null)
            {
                return null;
            }
            var teacher = _uofRepository.TeacherRepository.GetTeacherById(classSubjectList.TeacherID, ref dbFlag);
            
            if (teacher.SchoolID != _user.SchoolID)
            {
                sbError.Append("Teacher does not expst for school");
                return null;
            }
            
            return classSubjectList;
        }

        public List<TeacherClassSubjectDto> GetListTeacherClassSubjectsByTeacherIDandTermID(Guid teacherId, Guid termId, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var teacher = _uofRepository.TeacherRepository.GetTeacherById(teacherId, ref dbFlag);
            if (teacher == null)
            {
                return null;
            }

            if (teacher.SchoolID != _user.SchoolID)
            {
                sbError.Append("Teacher does not expst for school");
                return null;
            }

            var classSubjectList = _uofRepository.TeacherClassSubjectRepository.GetListTeacherClassSubjectsByTeacherIDandTermID(teacherId, termId, ref dbFlag);

            return classSubjectList;
        }

        

        public List<TeacherClassSubjectDto> GetCurrentTermTeacherClassSubjects( ref StringBuilder sbError)
        {
            bool dbFlag = false;
            return GetListTeacherClassSubjectsByTeacherIDandTermID(_user.TeacherID , _user.TermID , ref sbError);
           
        }

        public List<TeacherClassSubjectDto> GetListTeacherClassSubjectsByClassIDandTermID(Guid classId, Guid termId, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var teacher = _uofRepository.TeacherRepository.GetTeacherById(classId, ref dbFlag);
            if (teacher == null)
            {
                return null;
            }

            if (teacher.SchoolID != _user.SchoolID)
            {
                sbError.Append("Teacher does not exist for school");
                return null;
            }

            var classSubjectList = _uofRepository.TeacherClassSubjectRepository.GetListTeacherClassSubjectsByClassIDandTermID(classId, termId, ref dbFlag);

            return classSubjectList;
        }


        public List<TeacherClassSubjectDto> GetListByHeadOfDepartmentOrAdmin( ref StringBuilder sbError)
        {
            bool dbFlag = false;
            
            if (_user.isAdmin)
            {
                return _uofRepository.TeacherClassSubjectRepository.GetListTeacherClassSubjectsBySchoolIDandTermID(_user.SchoolID, _user.TermID, ref dbFlag);
            }

            var classSubjectList = _uofRepository.TeacherClassSubjectRepository.GetListForHeadOfDepartmentForATerm(_user.TeacherID , _user.TermID,_user.SchoolID , ref dbFlag);

            return classSubjectList;
        }




        public List<TeacherClassSubjectDto> GetListTeacherClassSubjectsBySchoolIDandTermID( Guid termId, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var teacher = _uofRepository.TermRepository.GetByID(termId, ref dbFlag);
            if (teacher == null)
            {
                return null;
            }

            if (teacher.SchoolID != _user.SchoolID)
            {
                sbError.Append("Term does not exist for school");
                return null;
            }

            var classSubjectList = _uofRepository.TeacherClassSubjectRepository.GetListTeacherClassSubjectsBySchoolIDandTermID(_user.SchoolID , termId, ref dbFlag);

            return classSubjectList;
        }

        public int SaveByTeacherClassSubject(TeacherClassSubjectSaveMethodClassListFORM formClassList, TeacherClassSubjectSaveMethodSubjectListFORM formSubjectList, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            int rowsAffected = 0;
            if (formSubjectList != null)
            {
                var classes = _uofRepository.ClassRepository.GetListClassesBySchoolID(_user.SchoolID, ref dbFlag);

                if (classes == null)
                {
                    return 0;
                }
                var classID = classes.Where(c => c.ClassID.Equals(formSubjectList.ClassID)).FirstOrDefault();
                if (classID == null)
                {
                    return 0;
                }


                foreach (var sub in (formSubjectList.SubjectList ?? new List<Guid>()))
                {
                    try
                    {
                        var isSaved = _uofRepository.TeacherClassSubjectRepository
                                                    .SaveTeacherClassSubjectsBySubject(formSubjectList.TeacherID, formSubjectList.ClassID, sub, _user.TermID, _user.Username, ref dbFlag);
                        if (isSaved)
                        {
                            rowsAffected++;
                        }
                    }
                    catch(Exception er)
                    {
                        sbError.Append("Error saving one record , this can be due to a duplicate of a deleted record for audit purpose");
                    }
                    
                }


            }
            else 
            { 
                /*
                 * 
                 * 
                 * Method 2
                 * 
                 * 
                 */ 

                var classes = _uofRepository.ClassRepository.GetListClassesBySchoolID(_user.SchoolID, ref dbFlag);

                if (classes == null || classes.Count() < 1)
                {
                    return 0;
                }
                

                foreach (var cl in (formClassList.ClassList ?? new List<Guid>()))
                {
                    var classID = classes.Where(c => c.ClassID.Equals(cl)).FirstOrDefault();
                    if (classID == null)
                    {
                        formClassList.ClassList.Remove(cl);
                    }
                    else if (classID.SchoolID != _user.SchoolID)
                    {
                        formClassList.ClassList.Remove(cl);
                    }
                    else
                    {
                        try
                        {
                            var isSaved = _uofRepository.TeacherClassSubjectRepository
                                                            .SaveTeacherClassSubjectsBySubject(formClassList.TeacherID, cl, formClassList.SubjectID, _user.TermID, _user.Username, ref dbFlag);
                            if (isSaved)
                            {
                                rowsAffected++;
                            }
                        }
                        catch (Exception er)
                        {
                            sbError.Append("Error saving one record , this can be due to a duplicate of a deleted record for audit purpose");
                        }
                    }
                }
            }
            return rowsAffected;   
        }
        
		public int ImportTeacherClassSubjectFromTermToAnotherTerm(Guid fromTermID , Guid toTermID , ref StringBuilder sbError)
		{
            if(fromTermID == toTermID)
            {
                sbError.Append("Term should be different");
                return 0;
            }
			var dbFlag = false;
			var schoolTermList = _uofRepository.TermRepository.GetListTermBySchoolID(_user.SchoolID, ref dbFlag);
			
			if(schoolTermList == null)
			{
				if((schoolTermList.Where( c => c.TermID == fromTermID).FirstOrDefault()) == null)
				{
					sbError.Append("School From Term Does Not Exist");
					return 0;
				} 
				if((schoolTermList.Where( c => c.TermID == toTermID).FirstOrDefault()) == null)
				{
					sbError.Append("School To Term Does Not Exist");
					return 0;
				} 
			}
			var listFromTerm = _uofRepository.TeacherClassSubjectRepository.GetListTeacherClassSubjectsBySchoolIDandTermID( _user.SchoolID , fromTermID, ref dbFlag);
			
			if(listFromTerm == null)
			{
				return 0;
			}
			
			List<TeacherClassSubject> listTransfered = new List<TeacherClassSubject>();
			foreach(var teacherClassSubject in listFromTerm)
			{
				listTransfered.Add
				(	new TeacherClassSubject()
					{ 
						TeacherClassSubjectID = null ,
						ClassID = teacherClassSubject.ClassID,
						TeacherID = teacherClassSubject.TeacherID,
						TermID = toTermID ,
						SubjectID = teacherClassSubject.SubjectID 
					}
				);
			} 
			return _uofRepository.TeacherClassSubjectRepository.SaveTeacherClassSubjectsBySubject(listTransfered, _user.Username, ref dbFlag);
        }
		
        public bool Delete(Guid TeacherClassSubjectID , ref StringBuilder  sbError)
        { 
            bool dbFlag = false;

            var exams = _uofRepository.ExamRepository.GetDeletedListExamByTeacherClassSubjectID(TeacherClassSubjectID, _user.SchoolID, ref dbFlag);
            if (dbFlag) { sbError.Append("Error getting  checking exams before delete"); }
            var tests = _uofRepository.TestRepository.GetListDtoTestByTeacherClassSubjectId(TeacherClassSubjectID, ref dbFlag);
            if (dbFlag) { sbError.Append("Error getting checking tests before delete"); }

            if (exams != null && exams?.Count() >= 1)
            {
                sbError.Append($"{exams?.Count()} exam records dependant on the record . ");
            }
            if (tests != null && tests?.Count() >= 1)
            {
                sbError.Append($"{tests?.Count()} tests records dependant on the record . ");
                return false;
            }
            
            return _uofRepository.TeacherClassSubjectRepository.Delete(TeacherClassSubjectID, _user.Username, ref dbFlag);
        }
    }
}
