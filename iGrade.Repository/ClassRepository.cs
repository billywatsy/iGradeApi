using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace iGrade.Repository
{
    public class ClassRepository : BaseRepository
    {

        public List<Class> GetListClassesBySchoolID(Guid schoolID, ref bool dbFlag)
        {
            var sql = @"SELECT  cl.* , lv.* , gr.*
                         FROM Class cl
                         inner join School sc on cl.SchoolID = sc.SchoolID
                         inner join Level lv on cl.LevelID = lv.LevelID
                         inner join Grade gr on cl.GradeID = gr.GradeID
                         WHERE sc.SchoolID = @schoolID 
                         AND cl.IsDeleted Is NULL 
                         AND sc.IsDeleted Is NULL 
                         AND lv.IsDeleted Is NULL 
                         AND gr.IsDeleted Is NULL ";

            using (var connection = GetConnection())
            {
                try
                {
                    var list = connection.Query<Class, Level, Grade, Class>(sql,
                           (clas, lv, gr) =>
                           {
                               clas.Level = lv;
                               clas.Grade = gr;
                               return clas;
                           }
                           , new { schoolID = schoolID }, splitOn: "LevelID,GradeID"
                               ).AsList();
                    return list;
                }
               catch(Exception er)
                {
                    return null;
                }
            }
        }

        public Class GetClassByID(Guid classID, ref bool dbFlag)
        {
            var sql = @"SELECT cl.* , sc.*  , lv.* , gr.*
                         FROM Class cl
                         inner join School sc on cl.SchoolID = sc.SchoolID
                         inner join Level lv on cl.LevelID = lv.LevelID
                         inner join Grade gr on cl.GradeID = gr.GradeID
                         WHERE cl.ClassID = @classID 
                         AND cl.IsDeleted Is NULL 
                         AND sc.IsDeleted Is NULL 
                         AND lv.IsDeleted Is NULL 
                         AND gr.IsDeleted Is NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<Class, Level, Grade , Class>(sql,
                            (clas, lv, gr) =>
                            {
                                clas.Level = lv;
                                clas.Grade = gr;
                                return clas;
                            }
                            , new { classID = classID }, splitOn: "LevelID,GradeID"
                                ).FirstOrDefault();
                return list;
            }
        }

        public Class Save(Class objClass, string modifiedBy , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    if (objClass.ClassID == null || Guid.Empty == objClass.ClassID)
                    {
                        objClass.ClassID = Guid.NewGuid();
                        objClass.ClassCode = CleanIDcodeAlphanumeric(objClass.ClassCode);
                        var update = @"
                                INSERT INTO Class
                                   (ClassID
                                   ,ClassName
                                   ,ClassCode
                                   ,LevelID
                                   ,SchoolID
                                   ,GradeID 
                                   ,LastModifiedBy
                                    )
                             VALUES
                                   (
                                    @id, 
                                    @Name , 
                                    @Code , 
                                    @LevelID  
                                   ,@SchoolID 
                                   ,@gradeID 
                                   ,@modifiedBy
                                  ) 
                                ";
                        var id = connection.Execute(update,
                                     new
                                     { 
                                         id = objClass.ClassID,
                                         LevelID = objClass.LevelID,
                                         Name = objClass.ClassName,
                                         Code = objClass.ClassCode,
                                         SchoolID = objClass.SchoolID,
                                         gradeID = objClass.GradeID,
                                         modifiedBy = modifiedBy
                                     });

                        return objClass;
                    }
                    else
                    {
                        var update = @"
                                UPDATE Class
					                            SET  ClassName = @Name   
					                             ,  ClassCode = @Code   
					                             ,  GradeID = @gradeID  
                                                 , LastModifiedBy = @modifiedBy
					                            WHERE ClassID = @ClassID
                                AND IsDeleted IS NULL
                                ";
                        var id = connection.Execute(update,
                                     new
                                     {
                                         ClassID = objClass.ClassID ,
                                         Name = objClass.ClassName,
                                         Code = objClass.ClassCode ,
                                         gradeID = objClass.GradeID,
                                         modifiedBy = modifiedBy
                                     });

                        return objClass;
                    }

                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return null;
            }
        }

        public bool Delete(Guid classId , string modifiedBy , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                { 
                    var update = @"  UPDATE  Class SET  isdeleted = now() , islive = null , LastModifiedBy = @modifiedBy WHERE ClassID = @ClassID AND IsDeleted IS NULL ;
                                ";
                    var id = connection.Query<int>(update, new
                                                          {
                                                               ClassID = classId , 
                                                               modifiedBy = modifiedBy
                    }).FirstOrDefault();
                    if(id > 0)
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
