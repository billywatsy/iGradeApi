using iGrade.Domain.Dto;
using iGrade.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class DashboardService
    {

        private iGrade.Repository.UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public DashboardService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user;
        }


        public DashboardCountDto GetDashboardCountDto(ref StringBuilder sbError)
        {
            bool dbError = false;
            var count = _uofRepository.DashboardRepository.GetDashboardCountBySchoolID(_user.SchoolID, _user.TermID, ref dbError) ?? new DashboardCountDto();
            return count;
        }


        public List<NumberRegisteredPerTermDto> GetTermAllTime(ref StringBuilder sbError)
        {
            bool dbError = false;
            var count = _uofRepository.DashboardRepository.GetNumberRegisteredBySchoolPerTerm(_user.SchoolID, ref dbError) ?? new List<NumberRegisteredPerTermDto>();
            return count;
        }
    }
}
