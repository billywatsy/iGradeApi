using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class ClassService : BaseService
    {
        private iGrade.Repository.UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public ClassService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public List<Class> GetClassesBySchoolId(ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.ClassRepository.GetListClassesBySchoolID(_user.SchoolID, ref dbFlag);
            if (dbFlag)
            {
                sbError.Append("errror getting classes");
            }
            return list;
        }

        public Class GetClassById(Guid classID , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var classValue = _uofRepository.ClassRepository.GetClassByID(classID, ref dbFlag);
            if (dbFlag)
            {
                sbError.Append("errror getting classes");
            }
            if (classValue != null)
            {
                if (classValue.SchoolID != _user.SchoolID)
                {
                    return null;
                }
            }
            return classValue;
        }
        public Class Save(Class classObject , ref StringBuilder sbError)
        {
            if(classObject == null)
            {
                sbError.Append("fill all fields");
                return null;
            }
            bool dbFlag = false;
            var allClasses = _uofRepository.ClassRepository.GetListClassesBySchoolID(_user.SchoolID, ref dbFlag) ?? new List<Class>();

            if (string.IsNullOrEmpty(classObject.ClassCode))
            {
                sbError.Append("class code is required");
                return null;
            }


            if (string.IsNullOrEmpty(classObject.ClassName))
            {
                sbError.Append("class name is required");
                return null;
            }


            if (classObject.ClassName.Length > 50)
            {
                sbError.Append("class name should be less than 50 characters");
                return null;
            }


            if (classObject.ClassCode.Length > 20)
            {
                sbError.Append("class name should be less than 20 characters");
                return null;
            }
            if (classObject.ClassID != null && classObject.ClassID != Guid.Empty)
            {
                var classValue = _uofRepository.ClassRepository.GetClassByID((Guid)classObject.ClassID, ref dbFlag);
                if (dbFlag)
                {
                    sbError.Append("Failed getting Class Details");
                    return null;
                }
                if(classValue == null)
                {
                    sbError.Append("school  does not have that class");
                    return null;
                }
                if (classValue.SchoolID != _user.SchoolID)
                {
                    sbError.Append("Class does  not belong to school");
                    return null;
                }
                

                classObject.LevelID = classValue.LevelID;
            }
            else
            {
                

                var isClassCodeExist = allClasses.Where(c => c.ClassCode.ToLower() == classObject.ClassCode.ToLower()).FirstOrDefault();

                if(isClassCodeExist != null)
                {
                    sbError.Append("class code already exist");
                    return null;
                }

                if (allClasses.Count() > iGrade.Core.TeacherUserService.Common.PolicyCommon.OveralClass)
                {
                    sbError.Append("You have reached maximum number of classes");
                    return null;
                }

                classObject.SchoolID = _user.SchoolID; 
            }
            var isSaved = _uofRepository.ClassRepository.Save(classObject, _user.Username , ref dbFlag);
            Log("Class", _user.TeacherID, $"saved class [ {classObject.ClassCode } ] details" , "SAVE");
            return isSaved;
           
        }

        public bool Delete(Guid id , ref StringBuilder sbError)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                sbError.Append("Failed getting class");
                return false ;
            }
            bool dbFlag = false;
            var classObject = _uofRepository.ClassRepository.GetClassByID(id , ref dbFlag);

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

            // is class having other dependants
            var classes = _uofRepository.ClassRepository.GetListClassesBySchoolID(_user.SchoolID ,ref dbFlag);

            if(classes == null)
            {
                sbError.Append("Failed geting classes for school");
            }
             
            var dependant = _uofRepository.ClassRepository.Delete((Guid)classObject.ClassID, _user.Username ,ref dbFlag);

            if (dependant)
            {

                Log("Class", _user.TeacherID, $" class [ {classObject.ClassCode } ] was removed", "DELETE");
                return true;
            }
            else
            {
                sbError.Append("failed deleting class");
                return false;
            }
        }
      
    }
}
 