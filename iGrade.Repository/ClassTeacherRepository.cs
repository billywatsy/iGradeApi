using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace iGrade.Repository
{
    public class ClassTeacherRepository : BaseRepository
    { 
        public List<ClassTeacher> GetListByTermID(Guid termID, Guid schoolID, ref bool dbFlag)
        {
            var sql = @"SELECT clt.* , cl.* , lv.* , te.*
                        FROM ClassTeacher clt
                        inner join Class cl on cl.ClassID = clt.ClassID
                        inner join School sc on cl.SchoolID = sc.SchoolID
                        inner join Level lv on cl.LevelID = lv.LevelID 
                        inner join Teacher te on clt.TeacherID = te.TeacherID 
                         WHERE cl.SchoolID = @schoolID
                         AND clt.TermID = @termID
                        AND sc.IsDeleted IS NULL
                        AND lv.IsDeleted IS NULL
                        AND te.IsDeleted IS NULL 
                        AND clt.IsDeleted IS NULL 
                        ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<ClassTeacher, Class, Level, Teacher, ClassTeacher>(sql,
                            (clasTea, clas, lv, tea) =>
                            {
                                clas.Level = lv;
                                clasTea.Class = clas;
                                clasTea.Teacher = tea;
                                return clasTea;
                            }
                            , new { schoolID = schoolID , termID = @termID }, splitOn: "ClassID,LevelID,TeacherID"
                                ).AsList();
                return list;
            }
        }
         
        public bool Save(ClassTeacher objClass, string modifiedBy ,ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                { 
                    var update = @"UPDATE ClassTeacher SET TeacherID = @TeacherID ,
                                    LastModifiedBy = @modifiedBy
                                                                    WHERE ClassID = @ClassID
                                                                    AND TermID = @TermID 
            
                                    AND IsDeleted Is NULL
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     ClassID = objClass.ClassID,
                                     TermID = objClass.TermID,
                                     TeacherID = objClass.TeacherID ,
                                     modifiedBy = modifiedBy ,
                                 });

                    if(id <= 0)
                    {
                        var insert = @" 
                                            INSERT INTO ClassTeacher
                                               (
                                                ClassID ,
                                                TermID ,
                                               TeacherID , 
                                                LastModifiedBy
                                                )
                                             VALUES
                                               (
                                                @ClassID,  
                                                @TermID , 
                                                @TeacherID  ,
                                                @modifiedBy
                                                ) 
                                ";
                        var idI = connection.Execute(insert,
                                     new
                                     {
                                         ClassID = objClass.ClassID,
                                         TermID = objClass.TermID,
                                         TeacherID = objClass.TeacherID , 
                                         modifiedBy = modifiedBy
                                     });

                    }
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

        public bool Delete(ClassTeacher classTeacher, string modifiedBy  , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                { 
                    var update = @"  
                                     UPDATE  ClassTeacher   SET  isdeleted = now() , islive = null ,
                                     LastModifiedBy = @modifiedBy
                                     where 
                                     ClassID = @ClassID 
                                     And TeacherID = @TeacherID 
                                     And TermID = @TermID 
                                     AND IsDeleted IS NULL
                                ";
                    var id = connection.Execute(update, new
                                                          {
                                                                ClassID = classTeacher.ClassID,
                                                                TermID = classTeacher.TermID,
                                                                TeacherID = classTeacher.TeacherID ,
                        modifiedBy = modifiedBy 
                    });
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
