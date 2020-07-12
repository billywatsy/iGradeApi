using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iGrade.Core.TeacherUserService.Common;

namespace iGrade.Core.TeacherUserService
{
    public class CommitteMemberService
    {
        private iGrade.Repository.UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public CommitteMemberService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public List<CommitteMember> GetListBySchoolId(ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.CommitteMemberRepository.GetListCommitteMemberBySchoolID(_user.SchoolID, ref dbFlag);
            if (dbFlag)
            {
                sbError.Append("errror getting classes");
            }
            return list;
        }

        public CommitteMember GetCommitteMemberByID(Guid committeMemberId , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var classValue = _uofRepository.CommitteMemberRepository.GetCommitteMemberByID(committeMemberId, ref dbFlag);
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
        public CommitteMember Save(CommitteMember committeMember , ref StringBuilder sbError)
        {
            if(committeMember == null)
            {
                sbError.Append("fill in all fields");
            }

            if(committeMember.CommitteMemberID == null)
            {
                committeMember.SchoolID = _user.SchoolID;
            }
            if (string.IsNullOrEmpty(committeMember.Title))
            {
                sbError.Append("Commitee member title does not exist");
            }

            if (string.IsNullOrEmpty(committeMember.Fullname))
            {
                sbError.Append("Commitee member Fullname is required");
            }

            if (!string.IsNullOrEmpty(committeMember.Phone))
            {
                if (!committeMember.Phone.IsPhoneValid(ref sbError))
                {
                    sbError.Append("Phone not valid");
                }
            }
            if (!string.IsNullOrEmpty(committeMember.Email))
            {
                if (!committeMember.Email.IsValidEmail())
                {
                    sbError.Append("Email is not valid"); 
                }
            }

            bool dbFlag = false;
            if(committeMember.CommitteMemberID != null && committeMember.CommitteMemberID != Guid.Empty)
            {
                var isExist = _uofRepository.CommitteMemberRepository.GetCommitteMemberByID((Guid)committeMember.CommitteMemberID , ref dbFlag);
                
                if(isExist == null)
                {
                    sbError.Append("Commitee Member does not exist");
                }
                else
                {
                    if(_user.SchoolID != isExist.SchoolID)
                    {
                        sbError.Append("Commitee Member does not belong to school");
                    }
                }
            }

            if (!string.IsNullOrEmpty(sbError.ToString()))
            {
                return null;
            }
            
            var isSaved = _uofRepository.CommitteMemberRepository.Save(committeMember, _user.Username , ref dbFlag);
             
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
            var committeMember = _uofRepository.CommitteMemberRepository.GetCommitteMemberByID(id , ref dbFlag);

            if (dbFlag)
            {
                sbError.Append("Error getting Committe Member");
                return false;
            }

            if (committeMember == null)
            {
                sbError.Append(" Committe Member does not exist");
                return false;
            }

            if(committeMember.SchoolID != _user.SchoolID)
            {
                sbError.Append("Error Committe Member does exist for school");
                return false;
            }
             
             
            var dependant = _uofRepository.CommitteMemberRepository.Delete((Guid)committeMember.CommitteMemberID , _user.Username ,ref dbFlag);

            if (dependant)
            {
                return true;
            }
            else
            {
                sbError.Append("failed deleting Committe Member ");
                return false;
            }
        }
      
    }
}
 