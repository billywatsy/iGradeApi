 
using iGrade.Domain;
using iGrade.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class LevelService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public LevelService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public List<Level> GetListLevelBySchoolID(ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.LevelRepository.GetListLevelsBySchoolID(_user.SchoolID, ref dbFlag);
            return list;
        }

        public Level GetLevelByLevelId(Guid levelId, ref StringBuilder sbError)
        {
            bool dbFlag = false;

            var level = _uofRepository.LevelRepository.GetLevelByLevelID(levelId, ref dbFlag);

            return level;
        }

        public Level Save(Level level, ref StringBuilder sbError)
        {

            if (level == null)
            {
                sbError.Append("fill all fields");
                return null;
            }
            
            if (string.IsNullOrEmpty(level.LevelCode))
            {
                sbError.Append("level code is required");
                return null;
            }


            if (string.IsNullOrEmpty(level.LevelName))
            {
                sbError.Append("level name is required");
                return null;
            }


            if (level.LevelName.Length > 50)
            {
                sbError.Append("level name should be less than 50 characters");
                return null;
            }


            if (level.LevelCode.Length > 20)
            {
                sbError.Append("level code should be less than 20 characters");
                return null;
            }
            var dbFlag = false;
            if (level.LevelID == null || Guid.Empty == level .LevelID)
            {
                level.SchoolID = _user.SchoolID;
            }
            else
            {

                var isLevelFromSchool = _uofRepository.LevelRepository.GetLevelByLevelID((Guid)level.LevelID, ref dbFlag);

                if(level.LevelID != isLevelFromSchool?.LevelID)
                {
                    sbError.Append("level not from school");
                    return null;
                }
                level.SchoolID = isLevelFromSchool.SchoolID;
            }
            

            var levels = this.GetListLevelBySchoolID(ref sbError) ?? new List<Level>();

            if (levels.Count() > 20)
            {
                sbError.Append("You have reached maximum number of levels allowed");
            }
            else
            {
                 if(level.LevelID != null)
                {
                    var dbLevel = levels.Where(c => c.LevelID == level.LevelID).FirstOrDefault();
                    if (dbLevel == null)
                    {
                        sbError.Append("Level does not exist for school");
                        return null;
                    }
                 }
                else
                {
                    var isLevelExist = levels.Where(c => c.LevelCode.ToLower() == level.LevelCode.ToLower()).FirstOrDefault();
                    if (isLevelExist != null)
                    {
                        sbError.Append("level code already exist");
                        return null;
                    }
                    level.SchoolID = _user.SchoolID;
                } 
                var save = _uofRepository.LevelRepository.Save(level, _user.Username, ref dbFlag);
                return save;
            }
            return null;
        }

        public bool Delete(Guid levelID, ref StringBuilder sbError)
        {

            var dbFlag = false;

            var level = _uofRepository.LevelRepository.GetLevelByLevelID(levelID, ref dbFlag);
            if (dbFlag)
            {
                sbError.Append("Failed getting level Details");
                return false;
            }
            if(level == null)
            {
                sbError.Append("Level does not exist");
                return false;
            }
            else
            {
                if(_user.SchoolID != level.SchoolID)
                {
                    sbError.Append("Level does not exist belong to school");
                    return false;
                }
            }

            if (!_user.isAdmin)
            {
                sbError.Append("You have to be admin to do this");
                return false;
            }

            var schoolClasses  = _uofRepository.ClassRepository.GetListClassesBySchoolID(_user.SchoolID, ref dbFlag);

            if (dbFlag)
            {
                sbError.Append("Failed getting classes for school");
                return false;
            }

            if (schoolClasses.Where(c => c.LevelID == levelID ).Count() >= 1) 
            {
                sbError.Append("Level can not be deleted as it is classes are dependant on it ");
            }
            else
            {
                var save = _uofRepository.LevelRepository.Delete((Guid)levelID, _user.Username,  ref dbFlag);
                return save;
            }
            return false;
        }

    }
}
 