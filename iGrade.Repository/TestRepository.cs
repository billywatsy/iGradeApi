using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using iGrade.Domain.Dto;

namespace iGrade.Repository
{
    public class TestRepository : BaseRepository
    {
        public Test GetTestByTestId(Guid testID, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT   te.* 
                                    FROM Test te
                                    INNER JOIN TeacherClassSubject tcs on te.TeacherClassSubjectID = tcs.TeacherClassSubjectID 
                                    inner join Class cla on cla.ClassID = tcs.ClassID
                                    inner join Subject sub on sub.SubjectId = tcs.SubjectId
                                    inner join Teacher tea on tea.TeacherID = tcs.TeacherID
                                    WHERE te.TestID = @testID 
                                    AND te.ISDELETED IS NULL 
                                    AND tcs.ISDELETED IS NULL 
                                    AND cla.ISDELETED IS NULL 
                                    AND sub.ISDELETED IS NULL 
                                    AND tea.ISDELETED IS NULL ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<Test>(sqlTestMark
                                ,
                                new
                                {
                                    TestID = testID
                                }
                                    ).FirstOrDefault();

                    return list;

                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
            }
            return null;
        }

        public List<TestDto> GetListDtoTestByTeacherClassSubjectId(Guid TeacherClassSubjectID, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT 
                             te.TestID ,
                             te.TeacherClassSubjectID ,
                             te.TestTitle ,
                             te.OutOf ,
                             te.TestDateCreated ,
                            (
                              SELECT Avg(Percentage) FROM TestMark Where TestMark.TestID = te.TestID AND ISDELETED IS NULL
                            ) As Average ,
                            (
                             SELECT Count(Percentage) FROM TestMark Where TestMark.TestID = te.TestID AND  ISDELETED IS NULL AND TestMark.Percentage > 49
                            ) As NumberPassed ,
                            (
                             SELECT Count(Percentage) FROM TestMark Where TestMark.TestID = te.TestID AND  ISDELETED IS NULL AND TestMark.Percentage < 50
                            ) As NumberFailed ,
                            (
                             SELECT Count(Percentage) FROM TestMark Where TestMark.TestID = te.TestID  AND ISDELETED IS NULL
                            ) As TotalWritten 
                             FROM 
                            Test te
                            Where te.teacherClassSubjectID = @tcsID AND te.ISDELETED IS NULL";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<TestDto>(sqlTestMark,
                                new
                                {
                                    // te.* , tcs.* , cla.* , sub.* , tea.*
                                    tcsID = TeacherClassSubjectID
                                }
                                    ).AsList();

                    return list;

                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
            }
            return null;
        }

        public List<TestDto> GetListTestBySubjectIdAndYear(Guid subjectID, int year , ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT 
                                        te.TestID ,
                                        te.TeacherClassSubjectID ,
                                        te.TestTitle ,
                                        te.OutOf ,
                                        te.TestDateCreated ,
                                    (
                                        SELECT Avg(Percentage) FROM TestMark Where TestMark.TestID = te.TestID  AND ISDELETED IS NULL
                                    ) As Average ,
                                    (
                                        SELECT Count(Percentage) FROM TestMark Where TestMark.TestID = te.TestID AND TestMark.Percentage > 49  AND ISDELETED IS NULL
                                    ) As NumberPassed ,
                                    (
                                        SELECT Count(Percentage) FROM TestMark Where TestMark.TestID = te.TestID AND TestMark.Percentage < 50 AND ISDELETED IS NULL
                                    ) As NumberFailed ,
                                    (
                                        SELECT Count(Percentage) FROM TestMark Where TestMark.TestID = te.TestID   AND ISDELETED IS NULL
                                    ) As TotalWritten 
                                        FROM 
                                    Test te
                                    Where te.TeacherClassSubjectID 
                                    IN
                                    (
                                        SELECT TeacherClassSubject.TeacherClassSubjectID FROM Term 
                                        INNER JOIN TeacherClassSubject on TeacherClassSubject.TermID = Term.TermID
                                        Where Year = @year
                                        And SubjectID =  @sid
                                        AND  TeacherClassSubject.ISDELETED IS NULL            
                                        AND  Term.ISDELETED IS NULL            
                                    ) AND te.ISDELETED IS NULL ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<TestDto>(sqlTestMark,
                                new
                                {
                                    // te.* , tcs.* , cla.* , sub.* , tea.*
                                    sid = subjectID ,
                                    year = year
                                }
                                    ).AsList();

                    return list;

                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
            }
            return null;
        }

        public Test Save(Test test, string modifiedby , ref bool dbError)
        {
            try
            {
                if (test.TestID == null ||  test.TestID == Guid.Empty)
                { 
                    var sql = @" 
            INSERT INTO Test
           (TestID
           ,TeacherClassSubjectID
           ,TestTitle
           ,OutOf
           ,TestDateCreated , Lastmodifiedby )
     VALUES
           (@id
           ,@TeacherClassSubjectID 
           ,@TestTitle 
           ,@OutOf 
           ,@dateCreated ,@modifiedby )";
                    var id = GetConnection().Execute(sql, new
                    {
                        id = Guid.NewGuid(),
                        TeacherClassSubjectID = test.TeacherClassSubjectID,
                        TestTitle = test.TestTitle,
                        OutOf = test.OutOf ,
                        dateCreated = test.TestDateCreated  ,
                        modifiedby = modifiedby
                    });
                }
                else
                {
                    var sql = @"UPDATE  Test
                                   SET 
                                      TestTitle = @TestTitle
                                      ,OutOf = @OutOf  , 
                                    LastModifiedBy = @modifiedby
                                 WHERE TestID = @TestID       ";
                    var id = GetConnection().Execute(sql, new
                    {
                        TestTitle = test.TestTitle,
                        OutOf = test.OutOf,
                        TestID = test.TestID ,
                        modifiedby = modifiedby
                    });
                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return null;
            }
            return test;
        }

        public bool Delete(Test test,string modifiedby , ref bool dbFlag)
        {
            using (var connection = GetConnection())
            {
                var update = @"UPDATE Test  SET  isdeleted = now() , islive = null , LASTMODIFIEDBY = @modifiedby WHERE TestID = @testID ; 
                                ";
                var id = connection.Execute(update,
                             new
                             {
                                 TestID = test.TestID ,
                                 modifiedby = modifiedby
                             });

                return true;

            }
        }
    }
}