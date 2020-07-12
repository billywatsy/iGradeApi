using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace iGrade.Repository
{
    public class SubjectRepository : BaseRepository
    {

        public List<Subject> GetListSubjects(Guid schoolId, ref bool dbFlag)
        {
            var sql = @"SELECT  *
                         FROM Subject
                        where SchoolID = @schoolID AND ISDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<Subject>(sql,
                                                new
                                                {
                                                    schoolID = schoolId
                                                }).AsList();
                return list;
            }
        }

        public Subject GetSubject(Guid subjectId, ref bool dbFlag)
        {
            var sql = @"SELECT   *
                         FROM Subject
                        where SubjectID = @subjectId AND ISDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<Subject>(sql,
                                                new
                                                {
                                                    subjectID = subjectId
                                                }).FirstOrDefault();
                return list;
            }
        }

        public Subject GetSubjectByCode(string subjectCode, Guid schoolId , ref bool dbFlag)
        {
            var sql = @"SELECT   *
                         FROM Subject
                        where SchoolID = @schoolId
                        AND SubjectCode = @subjectCode
AND ISDELETED IS NULL
                        ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<Subject>(sql,
                                                new
                                                {
                                                    subjectCode = subjectCode ,
                                                    schoolId = schoolId
                                                }).FirstOrDefault();
                return list;
            }
        }



        public bool Delete(Guid subjectId, string modifiedby , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @"UPDATE  Subject SET lastmodifiedby = @modifiedby ,  isdeleted = now() , islive = null  WHERE SubjectId = @subjectId 
                                
    AND SubjectID Not IN ( Select SubjectID FROM TeacherClassSubject where SubjectId = @subjectId AND  IsDeleted IS NULL)";
                    var id = connection.Execute(update, new
                    {
                        SubjectID = subjectId ,
                        modifiedby = modifiedby
                    });

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

        public Subject Save(Subject subject,string modifiedby , ref bool dbError)
        {

            var subjectIsExist = GetSubjectByCode(subject.SubjectCode, subject.SchoolID, ref dbError);
            if (dbError)
            {
                return null;
            }
            try
            {
                using (var connection = GetConnection())
                {
                    subject.SubjectCode = CleanIDcodeAlphanumeric(subject.SubjectCode);

               if (subjectIsExist == null)
                {
                        subject.SubjectID = Guid.NewGuid();
                        var insert = @"
                                INSERT INTO Subject
                                   (SubjectID
                                    ,DepartmentId
                                   ,SubjectName
                                   ,SubjectCode
                                   ,SchoolID  ,
  LastModifiedBy
                                    )
                             VALUES
                                   (
                                    @id , 
                                    @DepartmentId ,
                                    @name , 
                                    @code ,   
                                    @schoolID  ,@modifiedby 
                                    ) 
                                ";
                        var id = connection.Execute(insert,
                                     new
                                     {
                                         id = subject.SubjectID,
                                         DepartmentId = subject.DepartmentId ,
                                         code = subject.SubjectCode,
                                         name = subject.SubjectName,
                                         schoolID = subject.SchoolID ,
                                         modifiedby = modifiedby
                                     });
                    }
                else
                {

                        var update = @" 
                                UPDATE Subject
					                            SET  SubjectName = @name , 
                                                    DepartmentId = @DepartmentId , lastmodifiedby = @modifiedby    
					                            WHERE SubjectID = @id AND ISDELETED IS NULL
                                ";
                        var id = connection.Execute(update,
                                     new
                                     {
                                         id = subject.SubjectID,
                                         DepartmentId = subject.DepartmentId ,
                                         name = subject.SubjectName ,
                                         modifiedby = modifiedby
                                     });
                    }


                    return subject;

                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return null;
            }
        }
    }
}
