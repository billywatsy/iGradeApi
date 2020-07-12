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
    public class LogService
    {
        private UowRepository _uofRepository;
        private Domain.Dto.LoggedUser _user;
        public LogService(Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user;
        }

        public PagedList<LogDto> GetPagedList( int PageSize, int PageNo, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            PagedList<LogDto> list = _uofRepository.LogRepository.GetPagedLog(_user.SchoolID, PageSize, PageNo, ref dbFlag) ?? new PagedList<LogDto>();
            return list;
        }

        public PagedList<LogDto> GetPagedListTeacher( int PageSize, int PageNo, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            PagedList<LogDto> list = _uofRepository.LogRepository.GetPagedTeacherLog(_user.TeacherID, PageSize, PageNo, ref dbFlag) ?? new PagedList<LogDto>();
            return list;
        }

    }
}
