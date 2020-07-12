using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace iGrade.Repository
{
    public class TermRepository : BaseRepository
    {
        public Term Insert(Term term, string modifiedby , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    term.TermID = Guid.NewGuid();
                    var update = @"
                                INSERT INTO Term
                                   (
                                    TermID
                                    ,Year
                                   ,TermNumber
                                   ,SchoolID
                                   ,StartDate
                                   ,EndDate  , LAstModifiedBy
                                    )
                             VALUES
                                   (
                                    @id ,             
                                    @Year , 
                                    @TermNumber , 
                                    @SchoolID , 
                                    @FromDate ,   
                                    @ToDate  ,@modifiedby
                                    ) 
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     id = term.TermID,
                                     Year = term.Year,
                                     TermNumber = term.TermNumber,
                                     FromDate = term.StartDate,
                                     ToDate = term.EndDate,
                                     SchoolID = term.SchoolID , 
                                     modifiedby = modifiedby
                                 });

                    return term;

                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return null;
            }
        }

        public Term Update(Term term, string modifiedby ,ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @"
    UPDATE Term SET StartDate = @FromDate , EndDate = @ToDate , lastmodifiedby = @modifiedby WHERE TermID = @termID
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     termID = term.TermID,
                                     FromDate = term.StartDate,
                                     ToDate = term.EndDate , 
                                     modifiedby = modifiedby
                                 });

                    return term;

                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return null;
            }
        }

        public bool Delete(Guid termID, string modifiedby , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @"
    update Term set  isdeleted = now() , islive = null , lastmodifiedby =@modifiedby Where TermID = @termID
    AND TermID Not IN ( Select TermID FROM Setting where TermID = @termID  AND IsDeleted IS NULL)
    AND TermID Not IN ( Select TermID FROM TeacherClassSubject where TermID = @termID  AND IsDeleted IS NULL)
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     termID = termID ,
                                     modifiedby = modifiedby
                                 });

                    return true;

                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return false;
            }
        }
        public List<Term> GetListTermBySchoolID(Guid schoolID, ref bool dbFlag)
        {
            var sql = @"SELECT *
                         FROM Term  
                         WHERE SchoolID = @schoolID AND ISDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<Term>(sql
                            , new { schoolID = schoolID }
                                ).ToList();
                return list;
            }

        }
        public Term GetByID(Guid termID, ref bool dbFlag)
        {
            var sql = @"SELECT    *
                         FROM Term  
                         WHERE TermID = @termID AND ISDELETED IS NULL";

            using (var connection = GetConnection())
            {
                try
                {
                    var list = connection.Query<Term>(sql
                            , new { termID = termID }
                                ).FirstOrDefault();
                    return list;
                }
                catch (Exception er)
                {
                    dbFlag = true;
                    DbLog.Error(er);
                    return null;
                }

            }

            
        }
        public Term GetByTermNumberAndTermYearAndSchoolID(Guid schoolId, int termNumber, int termYear, ref bool dbFlag)
        {
            var sql = @"SELECT   *
                         FROM Term  
                         WHERE Year = @termYear
                         AND TermNumber = @termNumber
                         AND SchoolID = @schoolId AND ISDELETED IS NULL";

            using (var connection = GetConnection())
            {
                try
                {
                    var list = connection.Query<Term>(sql
                            , new { termYear = termYear, termNumber = termNumber, schoolId = schoolId }
                                ).FirstOrDefault();
                    return list;
                }
                catch (Exception er)
                {
                    dbFlag = true;
                    DbLog.Error(er);
                    return null;
                }

            }

        }
    }
}
