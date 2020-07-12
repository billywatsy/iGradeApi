using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class FeeTermService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public FeeTermService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user;
        }

        public List<FeeTerm> GetListByID(Guid termId , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.FeeTermRepository.GetListByTermID(termId, ref dbFlag);
            return list;
        }

        public FeeTerm GetByID(Guid feeTermId , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.FeeTermRepository.GetById(feeTermId, ref dbFlag);
            return list;
        }


        public bool Save(int year , int termNumber , string symbol , Guid feeTypeId , decimal fees, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var term = _uofRepository.TermRepository.GetByTermNumberAndTermYearAndSchoolID(_user.SchoolID  , termNumber, year, ref dbFlag);
            if(term == null)
            {
                sbError.Append("Term does not exist");
                return false;
            }
            FeeTerm feeTerm = new FeeTerm();
            feeTerm.TermID = (Guid)term.TermID;
            feeTerm.CurrencySymbol = symbol;
            feeTerm.Amount = fees;
            feeTerm.FeeTypeID = feeTypeId;
            return Save(feeTerm, ref sbError);
        }
        public bool Save(FeeTerm feeTerm, ref StringBuilder sbError)
        {
            var dbFlag = false;

            if(feeTerm == null)
            {
                sbError.Append("fill in all fields");
                return false;
            }

            var term = _uofRepository.TermRepository.GetByID(feeTerm.TermID, ref dbFlag);
            if (term == null)
            {
                sbError.Append("Term does not exist");
                return false;
            }

            if (term.Year >= (DateTime.Now.Year + 2))
            {

                sbError.Append("Enter a year less than :" + (DateTime.Now.Year + 2));
                return false;
            }
            if (term.Year <= (DateTime.Now.Year - 1))
            {

                sbError.Append("Enter a year greater than :" + (DateTime.Now.Year - 2));
                return false;
            }

            var feeType = _uofRepository.FeeTypeRepository.GetById(feeTerm.FeeTypeID, ref dbFlag);
            
            if (feeType == null)
            {
                sbError.Append("Fee type does not  exist");
                return false;
            }

            if(feeTerm.CurrencySymbol == null || feeTerm.CurrencySymbol?.Length >= 6)
            {
                sbError.Append("Fee term symbol should be less than 5 character");
                return false;
            }
            if (feeTerm.Amount <= 1)
            {
                sbError.Append("Fee amount should be greater than 1");
                return false;
            }

            if (feeTerm.Amount >= 10000000)
            {
                sbError.Append("Fee amount should be less than 10 000 000");
                return false;
            }
            var save = _uofRepository.FeeTermRepository.Save(feeTerm, _user.Username , ref dbFlag);
            return save;
        }

        public bool Delete(Guid feeTermId , ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var feeTerm = _uofRepository.FeeTermRepository.GetById(feeTermId, ref dbFlag);

            if(feeTerm == null)
            {
                sbError.Append("fee term does not exist ");
                return false;
            }
            return _uofRepository.FeeTermRepository.Delete(feeTermId, _user.Username , ref dbFlag);
        }
    }
}
