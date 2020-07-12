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
    public class ParentService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedParentUser _user;
        public ParentService(iGrade.Domain.Dto.LoggedParentUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user;
        }
         
    }
}
