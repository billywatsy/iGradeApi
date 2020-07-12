using Dapper;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Repository
{
    public class FeeTermRepository : BaseRepository
    {

        public bool Save(FeeTerm feeTerm, string modifiedBy , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    if (feeTerm.FeeTermID == null || Guid.Empty == feeTerm.FeeTermID)
                    {
                        var update = @"
                                INSERT INTO FeeTerm
           (FeeTermID
           ,TermID
           ,FeeTypeID
           ,CurrencySymbol
           ,Amount , lastmodifiedBy)
     VALUES
           (@id
           ,@TermID 
           ,@FeeTypeID
           ,@CurrencySymbol
           ,@Amount ,@modifiedBy)
                                ";
                        var id = connection.Execute(update,
                                     new
                                     {
                                        id = Guid.NewGuid() ,
                                         TermID = feeTerm.TermID,
                                         FeeTypeID = feeTerm.FeeTypeID,
                                         CurrencySymbol = feeTerm.CurrencySymbol,
                                         Amount = feeTerm.Amount ,
                                         modifiedBy = modifiedBy
                                     });

                        return true;
                    }
                    else
                    {
                        var update = @"
                                UPDATE FeeTerm
   SET  TermID = @TermID
      ,FeeTypeID = @FeeTypeID 
      ,CurrencySymbol = @CurrencySymbol 
      ,Amount = @Amount 
  ,LastmodifiedBy = @modifiedBy
 WHERE FeeTermID = @FeeTermID  AND ISDeleted IS NULL
                              ";
                        var id = connection.Execute(update,
                                     new
                                     {
                                         FeeTermID = feeTerm.FeeTermID,
                                         TermID = feeTerm.TermID,
                                         FeeTypeID = feeTerm.FeeTypeID,
                                         CurrencySymbol = feeTerm.CurrencySymbol,
                                         Amount = feeTerm.Amount ,
                                         modifiedBy = modifiedBy
                                     });

                        return true;
                    }

                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return false;
            }
        }

        public List<FeeTerm> GetListByTermID(Guid termID, ref bool dbFlag)
        {
            var sql = @"SELECT  FeeTerm.* , FeeType.*   FROM FeeTerm 
                        INNER JOIN FeeType on FeeTerm.FeeTypeID = FeeType.FeeTypeID
                        WHERE TermID = @termID AND FeeType.IsDEleted ISNULL AND FeeTerm.IsDeleted IS NULL ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<FeeTerm, FeeType , FeeTerm>(sql,
                    (fee, feeType) =>
                    {
                        fee.FeeType = feeType;
                        return fee;
                    } ,
                    new { termID = termID } , splitOn: "FeeTypeID")
                                 .AsList();
                return list;
            }
        }

        public FeeTerm GetById(Guid feeTermId, ref bool dbFlag)
        {
            var sql = @"SELECT * FROM FeeTerm 
                        INNER JOIN FeeType on FeeTerm.FeeTermID = FeeType.FeeTypeID
                        WHERE feeTermId = @feeTermId AND FeeType.IsDEleted ISNULL AND FeeTerm.IsDeleted IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<FeeTerm, FeeType, FeeTerm>(sql,
                    (fee, feeType) =>
                    {
                        fee.FeeType = feeType;
                        return fee;
                    },
                    new { feeTermId = feeTermId })
                                 .FirstOrDefault();
                return list;
            }
        }
        public bool Delete(Guid feeTermId,string modifiedBy ,  ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @" UPDATE feeTerm set lastmodifiedby = @modifiedBy ,  isdeleted = now() , islive = null   WHERE feeTermId = @feeTermId AND IsDeleted IS NULL ;
                                      
                                ";
                    var id = connection.Query<int>(update, new
                    {
                        feeTermId = feeTermId ,
                        modifiedBy = modifiedBy
                    }).FirstOrDefault();
                    if (id > 0)
                    {
                        return true;
                    }
                    return false;

                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return false;
            }
        }



        public List<FeeTerm> GetListByYearDifference(int startYear , int endYear , ref bool dbFlag)
        {
            var sql = @"SELECT  FeeTerm.* , FeeType.*   FROM FeeTerm 
                        INNER JOIN FeeType on FeeTerm.FeeTypeID = FeeType.FeeTypeID
                        INNER JOIN TERM on TERM.TermID = FeeTerm.TermID
                        WHERE TERM.YEAR >= @startYear AND TERM.YEAR <= @endYear
                        AND FeeType.IsDEleted ISNULL AND FeeTerm.IsDeleted IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<FeeTerm, FeeType, FeeTerm>(sql,
                    (fee, feeType) =>
                    {
                        fee.FeeType = feeType;
                        return fee;
                    },
                    new { startYear = startYear , endYear = endYear }, splitOn: "FeeTypeID")
                                 .AsList();
                return list;
            }
        }
    }
}
