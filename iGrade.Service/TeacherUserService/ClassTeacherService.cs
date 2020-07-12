using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class ClassTeacherService : BaseService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public ClassTeacherService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public List<ClassTeacher> GetListClassTeacherByTermId(Guid termID, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.ClassTeacherRepository.GetListByTermID(termID, _user.SchoolID, ref dbFlag);
            return list;
        }
        public List<ClassTeacher> GetListClassTeacherByCurrentTermId(ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.ClassTeacherRepository.GetListByTermID(_user.TermID, _user.SchoolID, ref dbFlag);
            return list;
        }

        public int Save(List<ClassTeacher> classTeacherList , ref StringBuilder sbError )
        {
            if(classTeacherList == null || classTeacherList.Count() <= 0)
            {
                sbError.Append("No data found");
                return 0;
            }


            bool dbFlag = false;
            var school = _uofRepository.SchoolRepository.GetSchoolBySchoolID(_user.SchoolID, ref dbFlag);
            if(school == null)
            {
                sbError.Append("School does not exist");
                return 0;
            }
            
            var allClasses = _uofRepository.ClassRepository.GetListClassesBySchoolID(_user.SchoolID, ref dbFlag);

            if(allClasses.Count() == 0)
            {
                sbError.Append("School does not have class");
                return 0;
            }

            int count = 0;
            foreach(var classTeacher in classTeacherList)
            {
                classTeacher.TermID = _user.TermID;
                var isSaved = InsertOrUpdate(classTeacher, ref sbError);
                if (isSaved)
                {
                    count++;
                }
            }
            return count;
        }

		public int ImportClassTeacherFromTermToAnotherTerm(Guid fromTermID , Guid toTermID , ref StringBuilder sbError)
		{
            if (fromTermID == toTermID)
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
			var listFromTerm = _uofRepository.ClassTeacherRepository.GetListByTermID(fromTermID ,_user.SchoolID  , ref dbFlag);
			
			if(listFromTerm == null)
			{
				return 0;
			}
			
			List<ClassTeacher> listTransfered = new List<ClassTeacher>();
			foreach(var classTeacher in listFromTerm)
			{
				listTransfered.Add
				(	new ClassTeacher()
					{ 
						ClassID = classTeacher.ClassID,
						TeacherID = classTeacher.TeacherID,
						TermID = toTermID , 
                        Teacher = _uofRepository.TeacherRepository.GetTeacherById(classTeacher.TeacherID , ref dbFlag) ,
                        Class = _uofRepository.ClassRepository.GetClassByID(classTeacher.ClassID , ref dbFlag)
					}
				);
			} 
			 
			var numberInserted = 0;
			
			foreach(var eachClasTeacher in listTransfered)
			{
				var save = _uofRepository.ClassTeacherRepository.Save(eachClasTeacher, _user.Username , ref dbFlag);
				if(save)
				{
                    Log("Class Teacher", _user.TeacherID, $"saved class key:  [ {eachClasTeacher?.Class.ClassCode} ] class key:  [ {eachClasTeacher?.Teacher?.TeacherUsername}  ] details", "SAVE");

                    numberInserted++;
				}
			}
			 
			return numberInserted;
		}
		
        private bool InsertOrUpdate(ClassTeacher classTeacher , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.ClassTeacherRepository.GetListByTermID(_user.TermID, _user.SchoolID, ref dbFlag);
            if (dbFlag)
            {
                sbError.Append("Error getting class teacher list if the problem persit ask system admin");
                return false;
            }
            if(list != null)
            {
                var classTeacherExist = list.Where(c => c.TeacherID == classTeacher.TeacherID).FirstOrDefault();

                if(classTeacherExist != null)
                {
                    sbError.Append("Teacher already has a class");
                    return false;
                }
            }

            var isSaved = _uofRepository.ClassTeacherRepository.Save(classTeacher, _user.Username , ref dbFlag);
            if (!isSaved)
            {
                sbError.Append("Error saving if the problem persit ask system admin");
                return false;
            }

            return true;
        }

         
        public bool Delete(Guid ClassID , Guid TeacherID , ref StringBuilder sbError)
        {
            if (string.IsNullOrEmpty(ClassID.ToString()))
            {
                sbError.Append("Failed getting class");
                return false ;
            }
            bool dbFlag = false;
            var classObject = _uofRepository.ClassRepository.GetClassByID(ClassID , ref dbFlag);

            if (dbFlag)
            {
                sbError.Append("Error getting class");
                return false;
            }

            if (classObject == null)
            {
                sbError.Append(" class does not exist");
                return false;
            }

            if(classObject.SchoolID != _user.SchoolID)
            {
                sbError.Append("Error class does exist for school");
                return false;
            }
            Log("Class Teacher", _user.TeacherID, $"removed class teacher for class code:  [ {classObject.ClassCode} ] ", "DELETE");

            return _uofRepository.ClassTeacherRepository.Delete(new ClassTeacher() { ClassID = (Guid)classObject.ClassID, TeacherID = TeacherID, TermID = _user.TermID }, _user.Username , ref dbFlag);
        }
      
    }
}
 