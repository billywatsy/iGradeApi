using Dapper;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Repository
{
    public class GradeMarkRepository : BaseRepository
    {

        public GradeMark Save(GradeMark grade, string modifiedBy , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    if (grade.GradeMarkID == null)
                    {
                        grade.GradeMarkID = Guid.NewGuid();
                        var update = @"
                                INSERT INTO GradeMark
           (GradeMarkID
           ,GradeID
           ,FromMark
           ,ToMark
           ,GradeValue
           ,DefaultComment , LastmodifiedBy)
     VALUES
           (@id
           ,@GradeID 
           ,@FromMark
           ,@ToMark
           ,@GradeValue
           , @DefaultComment , @modifiedBy)
                                ";
                        var id = connection.Execute(update,
                                     new
                                     {
                                         id = grade.GradeMarkID ,
                                         GradeID = grade.GradeID,
                                         FromMark = grade.FromMark,
                                         ToMark = grade.ToMark ,
                                         GradeValue = grade.GradeValue ,
                                         DefaultComment = grade.DefaultComment ,
                                         modifiedBy = @modifiedBy
                                     });
                        
                    }
                    else
                    {
                        var update = @"
                                UPDATE GradeMark
   SET  FromMark = @FromMark 
      ,ToMark = @ToMark
      ,GradeValue = @GradeValue 
      ,DefaultComment = @DefaultComment ,
lastmodifiedBy = @modifiedBy
 WHERE GradeMarkID = @GradeMarkID AND ISDELETED IS NULL
                                ";
                        var id = connection.Execute(update,
                                     new
                                     {
                                         FromMark = grade.FromMark,
                                         ToMark = grade.ToMark,
                                         GradeValue = grade.GradeValue,
                                         DefaultComment = grade.DefaultComment,
                                         GradeMarkID = grade.GradeMarkID,
                                         modifiedBy = modifiedBy
                                     });
                        
                    }
                    return grade;
                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return null;
            }
        }

        public List<GradeMark> GetGradeMarkListByGradeId(Guid gradeId, ref bool dbFlag)
        {
            var sql = @"SELECT *   FROM GradeMark WHERE GradeID = @gradeId ANd ISDELETED IS NULL ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<GradeMark>(sql, new { gradeId = gradeId })
                                 .AsList();
                return list;
            }
        }

        public GradeMark GetGradeMarkById(Guid gradeMarkId, ref bool dbFlag)
        {
            var sql = @"SELECT *   FROM GradeMark WHERE GradeMarkID = @gradeMarkId  ANd ISDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<GradeMark>(sql, new { gradeMarkId = gradeMarkId })
                                 .FirstOrDefault();
                return list;
            }
        }
        public bool Delete(Guid gradeMarkID, string  modifiedby , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @"UPDATE GradeMark SET lastmodifiedby = @modifiedby ,  isdeleted = now() , islive = null  where GradeMarkID = @GradeID  ANd ISDELETED IS NULL  ;
                                        Select 1;
                                ";
                    var id = connection.Query<int>(update, new
                    {
                        GradeID = gradeMarkID , 
                        modifiedby = modifiedby
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
