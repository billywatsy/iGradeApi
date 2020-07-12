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
    public class SchoolGroupService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public SchoolGroupService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user;
        }

        public SchoolGroup GetByID(Guid schoolGroupId, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.SchoolGroupRepository.GetSchoolGroupBySchoolID(schoolGroupId, ref dbFlag);
            return list;
        }

        public SchoolGroup GetByID(string groupCode, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.SchoolGroupRepository.GetSchoolGroupByCode(groupCode, ref dbFlag);
            return list;
        }
        
    }
}
