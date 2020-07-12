using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class SettingService
    {
        private UowRepository _uofRepository;
        private Domain.Dto.LoggedUser _user;

        public SettingService(Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user;
        }

        public Setting GetSchoolSetting(Guid schoolId, ref List<string> ltErrors)
        {
            bool dbFlag = false;
            var setting = _uofRepository.SettingRepository.GetSettingBySchoolID(schoolId, ref dbFlag);
            return setting;
        }

        public Setting Save(Setting settingForm, ref List<string> ltErrors)
        {
            if(settingForm == null)
            {
                ltErrors.Add("Form is required fill all fields");
                return null;
            }
            if(_user.SchoolID != settingForm.SchoolID)
            {
                ltErrors.Add("Form is required fill all fields for the school");
                return null;
            }
            if (!_user.isAdmin)
            {
                ltErrors.Add("you should be admin to save");
                return null;
            }

            bool dbFlag = false;
            var term = _uofRepository.TermRepository.GetByID(settingForm.TermID, ref dbFlag);

            if(term == null)
            {
                ltErrors.Add("term does not exist");
                return null;
            }
            if (_user.SchoolID != term.SchoolID)
            {
                ltErrors.Add("term does not belong to school");
            }
            if (_user.SchoolID != settingForm.SchoolID)
            {
                ltErrors.Add("term does not belong to school");
            }

            var currentTerm = _uofRepository.SettingRepository.GetSettingBySchoolID(_user.SchoolID, ref dbFlag);
            var termList = _uofRepository.TermRepository.GetListTermBySchoolID(_user.SchoolID, ref dbFlag);
            
             
          
            if (ltErrors != null)
            {
                if (ltErrors.Count() > 0)
                {
                    return null;
                }
            }

            var save = _uofRepository.SettingRepository.UpdateSetting(settingForm, _user.Username, ref dbFlag);
            
            var setting = _uofRepository.SettingRepository.GetSettingBySchoolID(_user.SchoolID, ref dbFlag);
            return setting;
        }

    }
}
