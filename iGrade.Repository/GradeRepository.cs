using Dapper;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Repository
{
    public class GradeRepository : BaseRepository
    {

        public Grade Save(Grade grade,string modifiedby , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    if (grade.GradeID == null || Guid.Empty == grade.GradeID)
                    {
                        grade.GradeID = Guid.NewGuid();
                        var update = @" 
INSERT INTO Grade
           (GradeID
           ,SchoolID 
           ,Description , lastmodifiedby)
     VALUES
           (
                                    @id
           ,@SchoolID 
           ,@Description , @modifiedby)
                                ";
                        var id = connection.Execute(update,
                                     new
                                     {
                                         id = Guid.NewGuid(),
                                         SchoolID = grade.SchoolID, 
                                         Description = grade.Description , 
                                         modifiedby = modifiedby
                                     });
                        
                    }
                    else
                    {
                        var update = @"
                                UPDATE Grade
                               SET  Description = @description , lastmodifiedby = @modifiedby
                             WHERE GradeID = @GradeID AND IsDeleted IS NULL
                                                            ";
                        var id = connection.Execute(update,
                                     new
                                     { 
                                         description = grade.Description,
                                         GradeID = grade.GradeID , 
                                         modifiedby = modifiedby
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

        public Grade GetGradeById(Guid gradeId , ref bool dbFlag)
        {
            var sql = @"SELECT *   FROM Grade  Where gradeId = @gradeId AND IsDeleted IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<Grade>(sql , new { gradeId  = gradeId })
                                 .FirstOrDefault();
                return list;
            }
        }


        public List<Grade> GetListBySchoolId( Guid schoolID , ref bool dbFlag)
        {
            var sql = @"SELECT * FROM Grade  Where schoolID = @schoolID AND ISDeleted IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<Grade>(sql , new { schoolID  = @schoolID })
                                 .AsList();
                return list;
            }
        }

        public bool Delete(Guid gradeID, string modifiedBy , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @"  UPDATE Grade SET lastmodifiedby = @modifiedBy  ,  isdeleted = now() , islive = null    WHERE GradeID = @GradeID AND ISDELETED IS NULL
                                AND GRADEID NOT IN (SELECT GRADEID FROM GRADEMARK WHERE GRADEID = @GradeID AND ISDELETED IS NULL ) ";
                    var id = connection.Query<int>(update, new
                    {
                        GradeID = gradeID ,
                        modifiedBy = modifiedBy
                    }).FirstOrDefault();
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
    }
}
