using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class SchoolInformationService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public SchoolInformationService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public SchoolInformation GetSchoolInfo()
        {
            bool dbFlag = false;
            var school = _uofRepository.SchoolRepository.GetSchoolBySchoolID(_user.SchoolID, ref dbFlag) ?? new School();
            var item = _uofRepository.SchoolInformationRepository.GetById(_user.SchoolID, ref dbFlag) ?? new SchoolInformation();
            return item;
        }

        public bool Save(SchoolInformation sch , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            
            if(sch == null)
            {
                sbError.Append("fill in all fields");
                return false;
            }

            sch.Lattitude = null;
            sch.SchoolId = _user.SchoolID;
            sch.Longitude = null;
            var isSaved = _uofRepository.SchoolInformationRepository.Save(sch, _user.Username, ref dbFlag);

            return isSaved;
        }
         
    }
}
 