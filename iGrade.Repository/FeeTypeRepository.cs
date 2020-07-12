using Dapper;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Repository
{
    public class FeeTypeRepository : BaseRepository
    {

        public bool Save(FeeType feeType,string modifiedBy , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    if (feeType.FeeTypeID == null || Guid.Empty == feeType.FeeTypeID)
                    {
                        var update = @" 
            INSERT INTO FeeType
                       (
                        FeeTypeID
                        ,SchoolID
                       ,Code 
                       ,Description , LastModifiedBy
                        )
                     VALUES
                           (
                             @id
                           ,@SchoolID 
                           ,@Code
                           ,@Description
 , @modifiedBy)
                                ";
                        var id = connection.Execute(update,
                                     new
                                     {
                                         id = Guid.NewGuid() ,
                                         SchoolID = feeType.SchoolID,
                                         Code = feeType.Code, 
                                         Description = feeType.Description ,
                                         modifiedBy = modifiedBy
                                     });

                        return true;
                    }
                    else
                    {
                        var update = @"
                                UPDATE FeeType
   SET  Code = @Code  ,
      Description = @Description ,
     LastmodifiedBy = now()
 WHERE FeeTypeID = @FeeTypeID AND IsDeleted IS NULL
                                ";
                        var id = connection.Execute(update,
                                     new
                                     {
                                         FeeTypeID = feeType.FeeTypeID, 
                                         Code = feeType.Code,
                                         Description = feeType.Description
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

        public FeeType GetById(Guid feeTypeId , ref bool dbFlag)
        {
            var sql = @"SELECT *   FROM FeeType  Where feeTypeId = @feeTypeId AND IsDeleted IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<FeeType>(sql , new { feeTypeId = feeTypeId })
                                 .FirstOrDefault();
                return list;
            }
        }


        public List<FeeType> GetListBySchoolId( Guid schoolID , ref bool dbFlag)
        {
            var sql = @"SELECT * FROM FeeType  Where schoolID = @schoolID  AND IsDeleted IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<FeeType>(sql , new { schoolID  = @schoolID })
                                 .AsList();
                return list;
            }
        }

        public bool Delete(Guid feeTypeId, string modifiedBy ,ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @"UPDATE FeeType SET lastmodifiedby = @modifiedBy  , isdeleted = now() , islive = null  WHERE feeTypeId = @feeTypeId  AND IsDeleted IS NULL
                                   ";
                    var id = connection.Query<int>(update, new
                    {
                        feeTypeId = feeTypeId ,
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
    }
}
