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
    public class FeeTypeService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public FeeTypeService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user;
        }

        public List<FeeType> GetListBySchoolID(ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.FeeTypeRepository.GetListBySchoolId(_user.SchoolID, ref dbFlag) ?? new List<FeeType>();
            return list;
        }

        public FeeType GetByID(Guid feeTypeId , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.FeeTypeRepository.GetById(feeTypeId, ref dbFlag);
            return list;
        }

        
        public bool Save(FeeType feeType, ref StringBuilder sbError)
        {

            if (feeType == null)
            {
                sbError.Append("enter all fields required");
                return false;
            }
            
            

            var dbFlag = false;

            var grades = _uofRepository.FeeTypeRepository.GetListBySchoolId(_user.SchoolID, ref dbFlag) ?? new List<FeeType>();

            if (feeType.FeeTypeID != null)
            {
                if (grades.Count() > 30)
                {
                    sbError.Append("You have reached maximum number of fee type allowed");
                    return false;
                }
            }
            
                feeType.SchoolID = _user.SchoolID;
            if (string.IsNullOrEmpty(feeType.Description))
            {
                sbError.Append("Description is required");
                return false;
            }
            if (feeType.Description.Length <= 1 || feeType.Description.Length >= 50)
            {
                sbError.Append("Description should not be long than 50 characters");
                return false;
            }
            if (string.IsNullOrEmpty(feeType.Code))
            {
                sbError.Append("Code is required");
                return false;
            }

            if (feeType.Code.Length <= 1 || feeType.Code.Length >= 10)
            {
                sbError.Append("Code should not be long than 10 characters");
                return false;
            }
            var save = _uofRepository.FeeTypeRepository.Save(feeType, _user.Username, ref dbFlag);
                return save;
            
        }

        public bool Delete(Guid feeTypeId , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            
            return _uofRepository.FeeTypeRepository.Delete(feeTypeId, _user.Username , ref dbFlag);
        }
    }
}
