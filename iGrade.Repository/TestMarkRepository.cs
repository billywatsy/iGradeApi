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
    public class TestMarkRepository : BaseRepository
    {
        public List<TestMarkDto> GetListByStudentID(Guid studentId, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
                                ex.TestID,
                                ex.studentTermRegisterID, 
                                stu.SchoolID,
                                tm.TermID ,
                                tm.Year ,
                                tm.TermNumber ,
                                sub.SubjectCode,
                                sub.SubjectName,
                                stu.RegNumber,
                                CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
                                ex.Mark,
                                ex.Percentage As MarkPercentage ,
                                tes.OutOf,
                                tes.TestTitle ,
                                tes.TestDateCreated
                        FROM TestMark ex
                        INNER JOIN Test  tes on tes.TestID = ex.TestID
                        INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = tes.TeacherClassSubjectID
                        INNER JOIN Term tm on tcs.TermID = tm.TermID
                        INNER JOIN Class cl on tcs.ClassID = cl.ClassID
                        INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
                        INNER JOIN StudentTermRegister enrol on enrol.studentTermRegisterID = ex.studentTermRegisterID 
                        INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
                        Where  enrol.StudentID = @studentId 
                        AND ex.ISDELETED IS NULL
                        AND tes.ISDELETED IS NULL
                        AND tcs.ISDELETED IS NULL
                        AND tm.ISDELETED IS NULL
                        AND cl.ISDELETED IS NULL
                        AND sub.ISDELETED IS NULL
                        AND enrol.ISDELETED IS NULL 
                        AND stu.ISDELETED IS NULL 
                            ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<TestMarkDto>(sqlTestMark
                                ,
                                new
                                {
                                    studentId = studentId
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
        public List<TestMarkDto> GetListTestMarksByTestId(Guid testID, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
                                ex.TestID,
                                ex.studentTermRegisterID, 
                                stu.SchoolID,
                                tm.TermID ,
                                tm.Year ,
                                tm.TermNumber ,
                                sub.SubjectCode,
                                sub.SubjectName,
                                stu.RegNumber,
                                CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
                                ex.Mark,
                                ex.Percentage As MarkPercentage ,
                                tes.OutOf,
                                tes.TestTitle ,
                                tes.TestDateCreated
                        FROM TestMark ex
                        INNER JOIN Test  tes on tes.TestID = ex.TestID
                        INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = tes.TeacherClassSubjectID
                        INNER JOIN Term tm on tcs.TermID = tm.TermID
                        INNER JOIN Class cl on tcs.ClassID = cl.ClassID
                        INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
                        INNER JOIN StudentTermRegister enrol on enrol.studentTermRegisterID = ex.studentTermRegisterID 
                        INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
                        Where  tes.TestId = @TestID 
                        AND ex.ISDELETED IS NULL
                        AND tes.ISDELETED IS NULL
                        AND tcs.ISDELETED IS NULL
                        AND tm.ISDELETED IS NULL
                        AND cl.ISDELETED IS NULL
                        AND sub.ISDELETED IS NULL
                        AND enrol.ISDELETED IS NULL 
                        AND stu.ISDELETED IS NULL 
                            ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<TestMarkDto>(sqlTestMark
                                ,
                                new
                                {
                                    TestID = testID
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

        public List<TestMarkDto> GetListTestMarksByClassIdAndTermID(Guid ClassId, Guid TermID ,ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
                                ex.TestID,
                                ex.studentTermRegisterID, 
                                stu.SchoolID,
                                tm.TermID ,
                                tm.Year ,
                                tm.TermNumber ,
                                sub.SubjectCode,
                                sub.SubjectName,
                                stu.RegNumber,
                                CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
                                ex.Mark,
                                ex.Percentage As MarkPercentage ,
                                tes.OutOf,
                                tes.TestTitle ,
                                tes.TestDateCreated
                        FROM TestMark ex
                        INNER JOIN Test  tes on tes.TestID = ex.TestID
                        INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = tes.TeacherClassSubjectID
                        INNER JOIN Term tm on tcs.TermID = tm.TermID
                        INNER JOIN Class cl on tcs.ClassID = cl.ClassID
                        INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
                        INNER JOIN StudentTermRegister enrol on enrol.studentTermRegisterID = ex.studentTermRegisterID 
                        INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
                        Where  tcs.ClassId = @ClassId AND tcs.TermID = @TermID 
                        AND ex.ISDELETED IS NULL
                        AND tes.ISDELETED IS NULL
                        AND tcs.ISDELETED IS NULL
                        AND tm.ISDELETED IS NULL
                        AND cl.ISDELETED IS NULL
                        AND sub.ISDELETED IS NULL
                        AND enrol.ISDELETED IS NULL 
                        AND stu.ISDELETED IS NULL 
                            ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<TestMarkDto>(sqlTestMark
                                ,
                                new
                                {
                                    TermID = TermID ,
                                    ClassId = ClassId 
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

        public List<TestMarkDto> GetListTestMarksByRegNumberAndSchoolID(string regNumber, Guid schoolID, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
        ex.TestID, 
        ex.studentTermRegisterID, 
        stu.SchoolID,
        tm.TermID ,
        tm.Year ,
        tm.TermNumber ,
        sub.SubjectCode,
        sub.SubjectName,
        stu.RegNumber,
        CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
        ex.Mark,
        ex.Percentage As MarkPercentage ,
        tes.OutOf ,
        tes.TestTitle , 
        tes.TestDateCreated
FROM TestMark ex
INNER JOIN Test  tes on tes.TestID = ex.TestID
INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = tes.TeacherClassSubjectID
INNER JOIN Term tm on tcs.TermID = tm.TermID
INNER JOIN Class cl on tcs.ClassID = cl.ClassID
INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
INNER JOIN StudentTermRegister enrol on enrol.studentTermRegisterID = ex.studentTermRegisterID 
INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
Where   stu.RegNumber = @regNumber 
And stu.SchoolID = @schoolID 
                        AND ex.ISDELETED IS NULL
                        AND tes.ISDELETED IS NULL
                        AND tcs.ISDELETED IS NULL
                        AND tm.ISDELETED IS NULL
                        AND cl.ISDELETED IS NULL
                        AND sub.ISDELETED IS NULL
                        AND enrol.ISDELETED IS NULL 
                        AND stu.ISDELETED IS NULL
                            ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<TestMarkDto>(sqlTestMark
                                ,
                                new
                                {
                                    schoolID = schoolID,
                                    regNumber = regNumber
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


        public List<TestMarkDto> GetListTestMarksByStudentID(Guid StudentID, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
        ex.TestID, 
        ex.studentTermRegisterID, 
        stu.SchoolID,
        tm.TermID ,
        tm.Year ,
        tm.TermNumber ,
        sub.SubjectCode,
        sub.SubjectName,
        stu.RegNumber,
        CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
        ex.Mark,
        ex.Percentage As MarkPercentage ,
        tes.OutOf ,
        tes.TestTitle , 
        tes.TestDateCreated
FROM TestMark ex
INNER JOIN Test  tes on tes.TestID = ex.TestID
INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = tes.TeacherClassSubjectID
INNER JOIN Term tm on tcs.TermID = tm.TermID
INNER JOIN Class cl on tcs.ClassID = cl.ClassID
INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
INNER JOIN StudentTermRegister enrol on enrol.studentTermRegisterID = ex.studentTermRegisterID 
INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
Where   stu.StudentID = @StudentID 
 
                        AND ex.ISDELETED IS NULL
                        AND tes.ISDELETED IS NULL
                        AND tcs.ISDELETED IS NULL
                        AND tm.ISDELETED IS NULL
                        AND cl.ISDELETED IS NULL
                        AND sub.ISDELETED IS NULL
                        AND enrol.ISDELETED IS NULL 
                        AND stu.ISDELETED IS NULL
                            ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<TestMarkDto>(sqlTestMark
                                ,
                                new
                                {
                                    StudentID = StudentID
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


        public List<TestMarkDto> GetListTestMarksByStudentIDAndTermID(Guid StudentID , Guid TermID ,  ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
        ex.TestID, 
        ex.studentTermRegisterID, 
        stu.SchoolID,
        tm.TermID ,
        tm.Year ,
        tm.TermNumber ,
        sub.SubjectCode,
        sub.SubjectName,
        stu.RegNumber,
        CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
        ex.Mark,
        ex.Percentage As MarkPercentage ,
        tes.OutOf ,
        tes.TestTitle , 
        tes.TestDateCreated
FROM TestMark ex
INNER JOIN Test  tes on tes.TestID = ex.TestID
INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = tes.TeacherClassSubjectID
INNER JOIN Term tm on tcs.TermID = tm.TermID
INNER JOIN Class cl on tcs.ClassID = cl.ClassID
INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
INNER JOIN StudentTermRegister enrol on enrol.studentTermRegisterID = ex.studentTermRegisterID 
INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
Where   stu.StudentID = @StudentID 
AND tm.TermID = @termID
                        AND ex.ISDELETED IS NULL
                        AND tes.ISDELETED IS NULL
                        AND tcs.ISDELETED IS NULL
                        AND tm.ISDELETED IS NULL
                        AND cl.ISDELETED IS NULL
                        AND sub.ISDELETED IS NULL
                        AND enrol.ISDELETED IS NULL 
                        AND stu.ISDELETED IS NULL
                            ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<TestMarkDto>(sqlTestMark
                                ,
                                new
                                {
                                    StudentID = StudentID ,
                                    termID = TermID
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

        public List<TestMarkDto> GetListTestMarksByStudentTermRegisterIDAndTermID(Guid studentTermRegisterID, Guid termId, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
        ex.TestID,
        ex.studentTermRegisterID, 
        stu.SchoolID,
        tm.TermID ,
        tm.Year ,
        tm.TermNumber ,
        sub.SubjectCode,
        sub.SubjectName,
        stu.RegNumber,
        CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
        ex.Mark, 
        ex.Percentage As MarkPercentage ,
        tes.OutOf,
        tes.TestTitle ,
        tes.TestDateCreated
FROM TestMark ex
INNER JOIN Test  tes on tes.TestID = ex.TestID
INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = tes.TeacherClassSubjectID
INNER JOIN Term tm on tcs.TermID = tm.TermID
INNER JOIN Class cl on tcs.ClassID = cl.ClassID
INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
INNER JOIN StudentTermRegister enrol on enrol.studentTermRegisterID = ex.studentTermRegisterID 
INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
Where   tcs.TermID = @termId 
And ex.studentTermRegisterID = @studentTermRegisterID 
                        AND ex.ISDELETED IS NULL
                        AND tes.ISDELETED IS NULL
                        AND tcs.ISDELETED IS NULL
                        AND tm.ISDELETED IS NULL
                        AND cl.ISDELETED IS NULL
                        AND sub.ISDELETED IS NULL
                        AND enrol.ISDELETED IS NULL 
                        AND stu.ISDELETED IS NULL
                            ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<TestMarkDto>(sqlTestMark
                                ,
                                new
                                {
                                    studentTermRegisterID = studentTermRegisterID,
                                    termId = termId
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


        public List<TestMarkDto> GetListTestMarksStudentIDAndYear(Guid studentId, int year, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
        ex.TestID,
        ex.studentTermRegisterID, 
        stu.SchoolID,
        tm.TermID ,
        tm.Year ,
        tm.TermNumber ,
        sub.SubjectCode,
        sub.SubjectName,
        stu.RegNumber,
        CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
        ex.Mark,
        ex.Percentage As MarkPercentage ,
        tes.OutOf,
        tes.TestTitle ,
         tes.TestDateCreated
FROM TestMark ex
INNER JOIN Test  tes on tes.TestID = ex.TestID
INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = tes.TeacherClassSubjectID
INNER JOIN Term tm on tcs.TermID = tm.TermID
INNER JOIN Class cl on tcs.ClassID = cl.ClassID
INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
INNER JOIN StudentTermRegister enrol on enrol.studentTermRegisterID = ex.studentTermRegisterID 
INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
Where   tm.Year = @year 
And stu.StudentID = @studentId 
 
                        AND ex.ISDELETED IS NULL
                        AND tes.ISDELETED IS NULL
                        AND tcs.ISDELETED IS NULL
                        AND tm.ISDELETED IS NULL
                        AND cl.ISDELETED IS NULL
                        AND sub.ISDELETED IS NULL
                        AND enrol.ISDELETED IS NULL 
                        AND stu.ISDELETED IS NULL
                            ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<TestMarkDto>(sqlTestMark
                                ,
                                new
                                {
                                    studentId = studentId,
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
        public List<TestMarkDto> GetListTestMarksStudentIDAndSubjectIDLatest100(Guid studentId, Guid subjectID, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
        ex.TestID,
        ex.studentTermRegisterID, 
        stu.SchoolID,
        tm.TermID ,
        tm.Year ,
        tm.TermNumber ,
        sub.SubjectCode,
        sub.SubjectName,
        stu.RegNumber,
        CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
        ex.Mark,
        ex.Percentage As MarkPercentage ,
        tes.OutOf,
        tes.TestTitle ,
        tes.TestDateCreated
FROM TestMark ex
INNER JOIN Test  tes on tes.TestID = ex.TestID
INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = tes.TeacherClassSubjectID
INNER JOIN Term tm on tcs.TermID = tm.TermID
INNER JOIN Class cl on tcs.ClassID = cl.ClassID
INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
INNER JOIN StudentTermRegister enrol on enrol.studentTermRegisterID = ex.studentTermRegisterID 
INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
Where   sub.SubjectID = @subjectID 
And stu.StudentID = @studentId 
                        AND ex.ISDELETED IS NULL
                        AND tes.ISDELETED IS NULL
                        AND tcs.ISDELETED IS NULL
                        AND tm.ISDELETED IS NULL
                        AND cl.ISDELETED IS NULL
                        AND sub.ISDELETED IS NULL
                        AND enrol.ISDELETED IS NULL 
                        AND stu.ISDELETED IS NULL
                        ORDER BY tes.TestDateCreated DESC LIMIT 100
                            ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<TestMarkDto>(sqlTestMark
                                ,
                                new
                                {
                                    studentId = studentId,
                                    subjectID = subjectID
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



        public List<TestMarkDto> GetListTestMarksStudentIDAndSubjectIDLatestByN(int numberLimit, Guid studentId, Guid subjectID, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
        ex.TestID,
        ex.studentTermRegisterID, 
        stu.SchoolID,
        tm.TermID ,
        tm.Year ,
        tm.TermNumber ,
        sub.SubjectCode,
        sub.SubjectName,
        stu.RegNumber,
        CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
        ex.Mark,
        ex.Percentage As MarkPercentage ,
        tes.OutOf,
        tes.TestTitle ,
        tes.TestDateCreated
FROM TestMark ex
INNER JOIN Test  tes on tes.TestID = ex.TestID
INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = tes.TeacherClassSubjectID
INNER JOIN Term tm on tcs.TermID = tm.TermID
INNER JOIN Class cl on tcs.ClassID = cl.ClassID
INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
INNER JOIN StudentTermRegister enrol on enrol.studentTermRegisterID = ex.studentTermRegisterID 
INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
Where   sub.SubjectID = @subjectID 
And stu.StudentID = @studentId 
                        AND ex.ISDELETED IS NULL
                        AND tes.ISDELETED IS NULL
                        AND tcs.ISDELETED IS NULL
                        AND tm.ISDELETED IS NULL
                        AND cl.ISDELETED IS NULL
                        AND sub.ISDELETED IS NULL
                        AND enrol.ISDELETED IS NULL 
                        AND stu.ISDELETED IS NULL
                        ORDER BY tes.TestDateCreated DESC LIMIT @numberLimit
                            ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<TestMarkDto>(sqlTestMark
                                ,
                                new
                                {
                                    studentId = studentId,
                                    subjectID = subjectID,
                                    numberLimit = numberLimit
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




        public List<Subject> GetUniqueSubjectsTestsWrittenByStudent( Guid studentId, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  *
FROM Subject Where SubjectID IN(
    SELECT SubjectId FROM testmark 
    INNER JOIN test on testmark.testid = test.testid
    INNER JOIN teacherclasssubject on teacherclasssubject.teacherclasssubjectId = test.teacherclasssubjectid
    INNER JOIN StudentTermRegister on StudentTermRegister.StudentTermRegisterID = testmark.StudentTermRegisterID
    WHERE 
    StudentTermRegister.StudentId =  @studentId
    AND test.IsDeleted IS NULL 
    AND testmark.IsDeleted IS NULL 
    AND Subject.IsDeleted IS NULL
    AND teacherclasssubject.IsDeleted IS NULL
    GROUP BY SubjectId
) ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<Subject>(sqlTestMark
                                ,
                                new
                                {
                                    studentId = studentId
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


        public List<TestMarkDto> GetTestMarkByDateRangeAndLevel(Guid schoolID , int numberOfRecords , int percentageStart , int percentageEnd , Guid levelID ,DateTime startDate , DateTime endDate , ref bool dbError)
        { 
            try
            {
                var sqlTestMark = @"SELECT  TOP( @numberOfRecords )  enrol.StudentID,
        ex.TestID,
        ex.studentTermRegisterID, 
        stu.SchoolID,
        tm.TermID ,
        tm.Year ,
        tm.TermNumber ,
        sub.SubjectCode,
        sub.SubjectName,
        stu.RegNumber,
        CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
        ex.Mark,
        ex.Percentage As MarkPercentage ,
        tes.OutOf,
        tes.TestTitle ,
        tes.TestDateCreated ,
        cl.ClassName , 
        lv.LevelName
FROM TestMark ex
INNER JOIN Test  tes on tes.TestID = ex.TestID
INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = tes.TeacherClassSubjectID
INNER JOIN Term tm on tcs.TermID = tm.TermID
INNER JOIN Class cl on tcs.ClassID = cl.ClassID
INNER JOIN Level lv on cl.LevelID = cl.LevelID
INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
INNER JOIN StudentTermRegister enrol on enrol.studentTermRegisterID = ex.studentTermRegisterID 
INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
Where lv.LevelID = @levelID 
And stu.SchoolID = @schoolID
AND tes.TestDateCreated BETWEEN @startDate AND @endDate
AND ex.Percentage BETWEEN @percentageStart AND @percentageEnd
 
                        AND ex.ISDELETED IS NULL
                        AND tes.ISDELETED IS NULL
                        AND tcs.ISDELETED IS NULL
                        AND tm.ISDELETED IS NULL
                        AND cl.ISDELETED IS NULL
                        AND sub.ISDELETED IS NULL
                        AND enrol.ISDELETED IS NULL 
                        AND stu.ISDELETED IS NULL";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<TestMarkDto>(sqlTestMark
                                ,
                                new
                                {
                                    schoolID = schoolID ,
                                    levelID = levelID,
                                    startDate = startDate,
                                    endDate = endDate,
                                    percentageStart = percentageStart ,
                                    percentageEnd = percentageEnd ,
                                    numberOfRecords = numberOfRecords
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


        public int CountAllTestWrittenByStudentID(Guid studentID, ref bool dbFlag)
        {
            var sql = @"SELECT COUNT(*)
                        FROM  TestMark ex
                        INNER JOIN  StudentTermRegister enrol on enrol.studentTermRegisterID = ex.studentTermRegisterID 
                        AND enrol.StudentID = @studentID AND ex.ISDeleted IS NULL AND enrol.IsDeleted IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Execute(sql, new { studentID = studentID });
                return list;
            }
        }


        public bool Save(TestMark test, string modifiedby , ref bool dbFlag)
        {
            using (var connection = GetConnection())
            {
                var update = @"
                                UPDATE TestMark
					                            SET  Mark = @Mark   
					                            ,  Percentage = @Percentage  , Lastmodifiedby = @modifiedby  
					                            WHERE studentTermRegisterID = @studentTermRegisterID
					                            AND TestID = @TestID
AND ISDELETED IS NULL
                                ";
                var id = connection.Execute(update,
                             new
                             {
                                 studentTermRegisterID = test.StudentTermRegisterID,
                                 TestID = test.TestID,
                                 Mark = test.Mark,
                                 Percentage = test.Percentage ,
                                 modifiedby = modifiedby
                             });
                if (id == 0)
                {
                    // insert
                    var insert = @"
                                INSERT INTO TestMark
                                   (studentTermRegisterID
                                   ,TestID
                                   ,Mark 
                                   ,Percentage , lastmodifiedby 
                                    )
                             VALUES
                                   (
                                    @studentTermRegisterID 
                                   ,@TestID 
                                   ,@Mark 
                                   ,@Percentage  , 
@modifiedby
                                    )
                                ";
                    id = connection.Execute(insert,
                                new
                                {
                                    studentTermRegisterID = test.StudentTermRegisterID,
                                    TestID = test.TestID,
                                    Mark = test.Mark,
                                    Percentage = test.Percentage ,
                                    modifiedby = modifiedby
                                });
                    
                }
                return true;
            }
        }
        public int Save(List<TestMark> testList, string modifiedby ,ref bool dbFlag)
        {
            int rowsAffected = 0;
            using (var connection = GetConnection())
            {
                foreach (var test in testList)
                {
                    var update = @"
                                UPDATE TestMark
					                            SET  Mark = @Mark   
					                            ,  Percentage = @Percentage    , Lastmodifiedby = @modifiedby 
					                            WHERE studentTermRegisterID = @studentTermRegisterID
					                            AND TestID = @TestID
AND ISDELETED IS NULL
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     studentTermRegisterID = test.StudentTermRegisterID,
                                     TestID = test.TestID,
                                     Mark = test.Mark ,
                                     Percentage = test.Percentage ,
                                     modifiedby = modifiedby
                                 } );
                    if(id == 0)
                    {
                        // insert
                        var insert = @"
                                INSERT INTO TestMark
                                   (studentTermRegisterID
                                   ,TestID
                                   ,Mark 
                                   ,Percentage 
                    ,Lastmodifiedby
                                    )
                             VALUES
                                   (
                                    @studentTermRegisterID 
                                   ,@TestID 
                                   ,@Mark 
                                   ,@Percentage , 
                                    @modifiedby
                                    )
                                ";
                         id = connection.Execute(insert,
                                     new
                                     {
                                         studentTermRegisterID = test.StudentTermRegisterID,
                                         TestID = test.TestID,
                                         Mark = test.Mark,
                                         Percentage = test.Percentage ,
                                         modifiedby = modifiedby
                                     });

                    }
                }

                return rowsAffected;
            }
        }
        public bool Delete(Guid studentTermRegisterID, Guid testID, string modifiedby ,ref bool dbFlag)
        {
            using (var connection = GetConnection())
            {

                var update = @"UPDATE TestMark SET  isdeleted = now() , islive = null , LASTMODIFIEDBY = @modifiedby WHERE studentTermRegisterID = @studentTermRegisterID
					                            AND TestID = @TestID AND  ISDELETED IS NULL  ; 
                                ";
                var id = connection.Execute(update,
                             new
                             {
                                 studentTermRegisterID = studentTermRegisterID,
                                 TestID = testID ,
                                 modifiedby = modifiedby
                             });

                return true;
            }
        }
    }
}