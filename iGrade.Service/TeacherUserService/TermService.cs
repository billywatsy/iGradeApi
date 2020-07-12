 
using iGrade.Domain;
using iGrade.Domain.Dto;
using iGrade.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class TermService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public TermService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public List<Term> GetListBySchoolID(ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.TermRepository.GetListTermBySchoolID( _user.SchoolID , ref dbFlag);
            if (dbFlag)
            {
                sbError.Append("Failed getting term list");
            }
            return list;
        }

        public Term GetTermId(Guid termID, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            return _uofRepository.TermRepository.GetByID(termID, ref dbFlag);
        }
        
        public Term Insert(Term term , ref List<string> ltErrors)
        {
            term.SchoolID = _user.SchoolID;
            bool dbFlag = false;

            var setting = _uofRepository.SettingRepository.GetSettingBySchoolID(_user.SchoolID, ref dbFlag);
            var termList = _uofRepository.TermRepository.GetListTermBySchoolID(_user.SchoolID, ref dbFlag);

            if (setting == null)
            {
                ltErrors.Add("Failed getting school setting");
                return null;
            }
             
            if(term.Year != term.EndDate.Year)
            {
                ltErrors.Add("year should be equal this year");
                return null;
            }
            if(term.Year != term.StartDate.Year)
            {
                ltErrors.Add("year should be equal this year");
                return null;
            }

            if(setting.MaximumTermPerYear <= 0)
            {
                ltErrors.Add("Please contact service provider to fix number of terms per year");
                return null;
            }
            else if(setting.MaximumTermPerYear >= 6)
            {
                ltErrors.Add("Please contact service provider to fix number of terms per year");
                return null;
            }

            if (termList != null)
            {
                var isTermExist = termList.Where(c => c.TermNumber == term.TermNumber)
                                           .Where(c => c.Year == term.Year)
                                           .FirstOrDefault();

                if (isTermExist != null)
                {
                    ltErrors.Add("Term Already exist");
                    return null;
                }
                else
                { 
                    if (term.TermNumber > setting.MaximumTermPerYear)
                    {
                        ltErrors.Add("Term number should be less than " + setting.MaximumTermPerYear);
                        return null;
                    }
                    if(term.TermNumber <= 0)
                    {
                        ltErrors.Add("Term number should be greater than  " + setting.MaximumTermPerYear);
                        return null;
                    }
                }
                
            }
             var termYearList =  termList.Where(c => c.Year == term.Year)
                                       .FirstOrDefault();

            if(term.TermNumber != 1)
            {
                var termNumberPrevious = term.TermNumber - 1;
                var isTermPrevious = termList.Where(c => c.TermNumber == termNumberPrevious)
                                           .Where(c => c.Year == term.Year)
                                           .FirstOrDefault();
                if(isTermPrevious == null)
                {
                    ltErrors.Add($"First add previous term {termNumberPrevious} to add your selected term");
                    return null;
                }
            }
            
            StringBuilder stringBuilder = new StringBuilder("");
            var isDateCorrect = IsTermToAndFromDateWithinRange(term.EndDate, term.StartDate, ref stringBuilder);

            if (!isDateCorrect)
            {
                ltErrors.Add(stringBuilder.ToString());
                return null;
            }

            var save = _uofRepository.TermRepository.Insert(term, _user.Username, ref dbFlag);

            return save;
        }

        public Term Save(Term term, ref StringBuilder sbError)
        {
            if (term.TermID == null)
            {
                term.SchoolID = _user.SchoolID;
                List<string> ltError = new List<string>();
                var isSaved = Insert(term , ref ltError);
                if (ltError != null)
                {
                    if (ltError.Count() > 0)
                    {
                        sbError.Append( string.Join(" , ", ltError.ToArray()));
                    }
                }
                return isSaved;
            }
            else
            {
                var isSaved = Update(term, ref sbError);
                return isSaved;
            }
        }

        public Term Update(Term term , ref StringBuilder sbError)
        {
            if(term.TermID == null)
            {
                sbError.Append("Term does not exist");
                return null;
            }

            var dbFlag = false;
            var termIsExist = _uofRepository.TermRepository.GetByID((Guid)term.TermID, ref dbFlag);

            if(termIsExist == null)
            {
                sbError.Append("Term does not exist");
                return null;
            }
            if(termIsExist.SchoolID != _user.SchoolID)
            {
                sbError.Append("Term does not exist");
                return null;
            }

            var isDateCorrect = IsTermToAndFromDateWithinRange(term.EndDate, term.StartDate , ref sbError);

            if (!isDateCorrect)
            {
                return null;
            }

            var save = _uofRepository.TermRepository.Update(term, _user.Username, ref dbFlag);
            return save;
        }

        private bool IsTermToAndFromDateWithinRange(DateTime toDate , DateTime fromDate , ref StringBuilder sbError)
        {
            if(toDate.Year != fromDate.Year)
            {
                sbError.Append("Year should be the same");
                return false;
            }

            if(toDate.Month <= fromDate.Month)
            {
                sbError.Append(" , To date month should be greater than from date month");
                return false;
            }

            TimeSpan diff = toDate.Subtract(fromDate);

            if (diff.Days <= 35)
            {
                sbError.Append("Difference between dates should be more than 35 days");
                return false;
            }
            else if (diff.Days >= 200)
            {
                sbError.Append("Difference between dates should be less than 200 days");
                return false;
            }

            return true;
        }

        public bool Delete (Guid termID , ref StringBuilder sbError)
        {
            var dbFlag = false;
            var isTerm = _uofRepository.TeacherClassSubjectRepository.GetListTeacherClassSubjectsBySchoolIDandTermID(_user.SchoolID, termID, ref dbFlag);

            if (dbFlag)
            {
                sbError.Append("Error occured");
                return false;
            }
            if(isTerm != null)
            {
                if(isTerm.Count() > 0)
                {
                    sbError.Append("Term can not be deleted");
                    return false;
                }
            }

            var delete = _uofRepository.TermRepository.Delete(termID, _user.Username, ref dbFlag);

            return delete;
        }
    }
}
 