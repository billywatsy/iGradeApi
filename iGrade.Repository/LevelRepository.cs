using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace iGrade.Repository
{
    public class LevelRepository : BaseRepository
    {
        public bool Delete(Guid levelId, string modifiedby , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @" UPDATE  Level SET lastmodifiedby = @modifiedby , isdeleted = now() , islive = null  WHERE LevelId = @levelId
                                    AND LevelID Not IN (Select LevelID FROM Class WHERE LevelID = @levelId AND IsDeleted IS NULL); 
                                ";
                    var id = connection.Query<int>(update, new
                    {
                        levelId = levelId ,
                        modifiedby = modifiedby
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

        public Level GetLevelByLevelID(Guid levelID, ref bool dbFlag)
        {
            var sql = @"SELECT ex.* 
                        FROM Level ex 
                        where ex.LevelID =  @levelID
AND IsDeleted IS NULL
                          ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<Level>(sql,
                                                new
                                                {
                                                    levelID = levelID
                                                })
                                 .FirstOrDefault();
                return list;
            }
        }

        public List<Level> GetListLevelsBySchoolID(Guid schoolID, ref bool dbFlag)
        {
            var sql = @"SELECT ex.*
                           FROM Level ex 
                          where ex.schoolID = @schoolID
AND IsDeleted IS NULL
                          ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<Level>(sql,
                                                new
                                                {
                                                    schoolID = schoolID
                                                }
                                                )
                                 .AsList();
                return list;
            }
        }

        public Level GetLevelsBySchoolIDandLevelName(Guid schoolID, string LevelName, ref bool dbFlag)
        {
            var sql = @"SELECT ex.*
                           FROM Level ex 
                          where ex.schoolID = @schoolID
                          And ex.LevelName = @levelName 
AND IsDeleted IS NULL
                          ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<Level>(sql,
                                                new
                                                {
                                                    schoolID = schoolID
                                                }
                                                )
                                 .FirstOrDefault();
                return list;
            }
        }

        public Level Save(Level level, string modifiedBy ,ref bool dbFlag)
        {
            using (var connection = GetConnection())
            {
                if(level.LevelID == null || level.LevelID == Guid.Empty)
                {
                    level.LevelID = Guid.NewGuid();
                    level.LevelCode = CleanIDcodeAlphanumeric(level.LevelCode);
                    var update = @"
                                INSERT INTO Level
                                   (LevelID
                                   ,LevelName
                                   ,LevelCode
                                   ,SchoolID ,lastmodifiedBy
                                    )
                             VALUES
                                   (
                                    @id , 
                                    @LevelName , 
                                    @Code , 
                                    @SchoolID ,@modifiedBy
                                   ) 
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     id = level.LevelID,
                                     LevelName = level.LevelName,
                                     Code = level.LevelCode,
                                     SchoolID = level.SchoolID ,
                                     modifiedBy = modifiedBy
                                 });

                    return level;
                }
                else
                {

                    var update = @"         UPDATE Level
					                        SET  LevelName = @LevelName   
					                        ,  LevelCode = @Code  ,
                                            lastmodifiedBy = @modifiedBy 
					                        WHERE LevelID = @LevelID 
                                            AND SchoolID = @SchoolID 
AND ISDeleted IS NULL
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     LevelID = level.LevelID,
                                     Code = level.LevelCode,
                                     LevelName = level.LevelName ,
                                     SchoolID = level.SchoolID ,
                                     modifiedBy = modifiedBy
                                 });

                    return level;
                }

            }

        }

    }
}
