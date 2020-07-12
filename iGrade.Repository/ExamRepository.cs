using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using iGrade.Domain.Dto;

namespace iGrade.Repository
{
    public class ExamRepository : BaseRepository
    {
        public List<ExamDto> GetListByLevelIDandTermID(Guid schoolID, Guid levelID, Guid termID, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
                                            ex.TeacherClassSubjectID,
                                            ex.StudentTermRegisterID, 
                                            stu.SchoolID,
                                            tm.TermID ,
                                            tm.Year ,
                                            tm.TermNumber ,
                                            sub.SubjectCode,
                                            sub.SubjectName,
                                            stu.RegNumber,
                                            CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
                                            ex.Mark,
                                            ex.Grade, 
                                            stu.IsMale ,
                                            cl.ClassName ,
                                            lv.LevelName ,
                                            tm.endDate AS EndOfTermDate 
                                    FROM Exam ex
                                    INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = ex.TeacherClassSubjectID
									INNER JOIN Term tm on tcs.TermID = tm.TermID
									INNER JOIN Class cl on tcs.ClassID = cl.ClassID
									INNER JOIN Level lv on lv.LevelID = cl.LevelID
                                    INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
                                    INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ex.StudentTermRegisterID 
                                    INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
                                    Where stu.SchoolID = @schoolID
									AND cl.LevelID = @levelID
									AND lv.LevelID = @levelID
									AND tcs.TermID = @termID 
                                    AND stu.IsDeleted IS NULL
									AND cl.IsDeleted IS NULL
									AND lv.IsDeleted IS NULL
									AND tcs.IsDeleted IS NULL 
									AND ex.IsDeleted IS NULL 
									AND tm.IsDeleted IS NULL 
									AND sub.IsDeleted IS NULL 
									AND enrol.IsDeleted IS NULL  ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<ExamDto>(sqlTestMark,
                                new
                                {
                                    levelID = levelID,
                                    termID = termID,
                                    schoolID = schoolID
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


        public List<ExamDto> GetListByLevelIDandTermIDAndSubjectIDs(Guid schoolID, Guid levelID, Guid termID,List<Guid> subjectIDs , ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
                                            ex.TeacherClassSubjectID,
                                            ex.StudentTermRegisterID, 
                                            stu.SchoolID,
                                            tm.TermID ,
                                            tm.Year ,
                                            tm.TermNumber ,
                                            sub.SubjectCode,
                                            sub.SubjectName,
                                            stu.RegNumber,
                                            CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
                                            ex.Mark,
                                            ex.Grade, 
                                            stu.IsMale ,
                                            cl.ClassName ,
                                            lv.LevelName ,
                                            tm.endDate AS EndOfTermDate 
                                    FROM Exam ex
                                    INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = ex.TeacherClassSubjectID
									INNER JOIN Term tm on tcs.TermID = tm.TermID
									INNER JOIN Class cl on tcs.ClassID = cl.ClassID
									INNER JOIN Level lv on lv.LevelID = cl.LevelID
                                    INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
                                    INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ex.StudentTermRegisterID 
                                    INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
                                    Where stu.SchoolID = @schoolID
									AND cl.LevelID = @levelID
									AND lv.LevelID = @levelID
									AND tcs.TermID = @termID 
                                    AND sub.SubjectID IN @subjectIDs
                                    AND stu.IsDeleted IS NULL
									AND cl.IsDeleted IS NULL
									AND lv.IsDeleted IS NULL
									AND tcs.IsDeleted IS NULL 
									AND ex.IsDeleted IS NULL 
									AND tm.IsDeleted IS NULL 
									AND sub.IsDeleted IS NULL 
									AND enrol.IsDeleted IS NULL  ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<ExamDto>(sqlTestMark,
                                new
                                {
                                    levelID = levelID,
                                    termID = termID,
                                    schoolID = schoolID ,
                                    subjectIDs = subjectIDs
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


        public List<ExamDto> GetListByStudentID(Guid studentID, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
                                            ex.TeacherClassSubjectID,
                                            ex.StudentTermRegisterID, 
                                            stu.SchoolID,
                                            tm.TermID ,
                                            tm.Year ,
                                            tm.TermNumber ,
                                            sub.SubjectCode,
                                            sub.SubjectName,
                                            stu.RegNumber,
                                            CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
                                            ex.Mark,
                                            ex.Grade,
                                            ex.Comment, 
                                            stu.IsMale ,
                                            cl.ClassName ,
                                            lv.LevelName , 
                                            tm.endDate AS EndOfTermDate 
                                    FROM Exam ex
                                    INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = ex.TeacherClassSubjectID
									INNER JOIN Term tm on tcs.TermID = tm.TermID
									INNER JOIN Class cl on tcs.ClassID = cl.ClassID
									INNER JOIN Level lv on lv.LevelID = cl.LevelID
                                    INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
                                    INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ex.StudentTermRegisterID 
                                    INNER JOIN Student stu on stu.StudentID = enrol.StudentID   
									Where stu.StudentID = @studentID 
                                    AND stu.IsDeleted IS NULL
									AND cl.IsDeleted IS NULL
									AND lv.IsDeleted IS NULL
									AND tcs.IsDeleted IS NULL 
									AND ex.IsDeleted IS NULL 
									AND tm.IsDeleted IS NULL 
									AND sub.IsDeleted IS NULL 
									AND enrol.IsDeleted IS NULL 
                ORDER BY tm.Year DESC, tm.TermNumber ASC
									";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<ExamDto>(sqlTestMark,
                                new
                                {
                                    studentID = studentID
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


        public List<ExamDto> GetListByStudentIDAndTermID(Guid studentID, Guid termID, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
                                            ex.TeacherClassSubjectID,
                                            ex.StudentTermRegisterID, 
                                            stu.SchoolID,
                                            tm.TermID ,
                                            tm.Year ,
                                            tm.TermNumber ,
                                            sub.SubjectCode,
                                            sub.SubjectName,
                                            stu.RegNumber,
                                            CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
                                            ex.Mark,
                                            ex.Grade,
                                            ex.Comment, 
                                            stu.IsMale ,
                                            cl.ClassName ,
                                            lv.LevelName ,
                                            tm.endDate AS EndOfTermDate 
                                    FROM Exam ex
                                    INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = ex.TeacherClassSubjectID
									INNER JOIN Term tm on tcs.TermID = tm.TermID
									INNER JOIN Class cl on tcs.ClassID = cl.ClassID
									INNER JOIN Level lv on lv.LevelID = cl.LevelID
                                    INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
                                    INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ex.StudentTermRegisterID 
                                    INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
                                    Where tm.TermID = @termID
									AND stu.StudentID = @studentID 
                                    AND stu.IsDeleted IS NULL
									AND cl.IsDeleted IS NULL
									AND lv.IsDeleted IS NULL
									AND tcs.IsDeleted IS NULL 
									AND ex.IsDeleted IS NULL 
									AND tm.IsDeleted IS NULL 
									AND sub.IsDeleted IS NULL 
									AND enrol.IsDeleted IS NULL 
									ORDER BY tm.Year DESC, tm.TermNumber ASC
									";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<ExamDto>(sqlTestMark,
                                new
                                {
                                    studentID = studentID,
                                    termID = termID
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

        public List<ExamDto> GetListByYearAndStudentID(Guid studentID, int year , ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
                                            ex.TeacherClassSubjectID,
                                            ex.StudentTermRegisterID, 
                                            stu.SchoolID,
                                            tm.TermID ,
                                            tm.Year ,
                                            tm.TermNumber ,
                                            sub.SubjectCode,
                                            sub.SubjectName,
                                            stu.RegNumber,
                                            CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
                                            ex.Mark,
                                            ex.Grade,
                                            ex.Comment, 
                                            stu.IsMale ,
                                            tm.endDate AS EndOfTermDate 
                                    FROM Exam ex
                                    INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = ex.TeacherClassSubjectID
									INNER JOIN Term tm on tcs.TermID = tm.TermID
									INNER JOIN Class cl on tcs.ClassID = cl.ClassID
                                    INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
                                    INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ex.StudentTermRegisterID 
                                    INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
                                    Where tm.Year = @year
									AND stu.StudentID = @studentID 
                                    AND stu.IsDeleted IS NULL
									AND cl.IsDeleted IS NULL 
									AND tcs.IsDeleted IS NULL 
									AND ex.IsDeleted IS NULL 
									AND tm.IsDeleted IS NULL 
									AND sub.IsDeleted IS NULL 
									AND enrol.IsDeleted IS NULL 
									ORDER BY tm.Year DESC, tm.TermNumber ASC
									";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<ExamDto>(sqlTestMark,
                                new
                                {
                                    studentID = studentID,
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

        public List<ExamDto> GetListByClassIDandTermID(Guid schoolID , Guid classID , Guid termID , ref bool dbError)
		{
			try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
                                            ex.TeacherClassSubjectID,
                                            ex.StudentTermRegisterID, 
                                            stu.SchoolID,
                                            tm.TermID ,
                                            tm.Year ,
                                            tm.TermNumber ,
                                            sub.SubjectCode,
                                            sub.SubjectName,
                                            stu.RegNumber,
                                            CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
                                            ex.Mark,
                                            ex.Grade,
                                            ex.Comment, 
                                            stu.IsMale ,
                                            tm.endDate AS EndOfTermDate 
                                    FROM Exam ex
                                    INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = ex.TeacherClassSubjectID
									INNER JOIN Term tm on tcs.TermID = tm.TermID
                                    INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
                                    INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ex.StudentTermRegisterID 
                                    INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
                                    Where stu.SchoolID = @schoolID
									AND tcs.ClassID = @classID
									AND tcs.TermID = @termID
                                    AND stu.IsDeleted IS NULL 
									AND tcs.IsDeleted IS NULL 
									AND ex.IsDeleted IS NULL 
									AND tm.IsDeleted IS NULL 
									AND sub.IsDeleted IS NULL 
									AND enrol.IsDeleted IS NULL  ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<ExamDto>(sqlTestMark,
                                new
                                {
                                    classID = classID,
									termID = termID ,
                                    schoolID = schoolID
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
        public List<ExamDto> GetListExamByTeacherClassSubjectID(Guid teacherClassSubjectID, Guid schoolID, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
                                            ex.TeacherClassSubjectID,
                                            ex.StudentTermRegisterID, 
                                            stu.SchoolID,
                                            tm.TermID ,
                                            tm.Year ,
                                            tm.TermNumber ,
                                            sub.SubjectCode,
                                            sub.SubjectName,
                                            stu.RegNumber,
                                            CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
                                            ex.Mark,
                                            ex.Grade,
                                            ex.Comment, 
                                            stu.IsMale ,
                                            tm.endDate AS EndOfTermDate 
                                    FROM Exam ex
                                    INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = ex.TeacherClassSubjectID
									INNER JOIN Term tm on tcs.TermID = tm.TermID
                                    INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
                                    INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ex.StudentTermRegisterID 
                                    INNER JOIN Student stu on stu.StudentID = enrol.StudentID 
                                    Where ex.TeacherClassSubjectID = @teacherClassSubjectID
                                    AND stu.SchoolID = @schoolID
                                    AND stu.IsDeleted IS NULL 
									AND tcs.IsDeleted IS NULL 
									AND ex.IsDeleted IS NULL 
									AND tm.IsDeleted IS NULL 
									AND sub.IsDeleted IS NULL 
									AND enrol.IsDeleted IS NULL  ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<ExamDto>(sqlTestMark,
                                new
                                {
                                    TeacherClassSubjectID = teacherClassSubjectID,
                                    schoolID = schoolID
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

        public List<ExamDto> GetDeletedListExamByTeacherClassSubjectID(Guid teacherClassSubjectID, Guid schoolID, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
                                            ex.TeacherClassSubjectID,
                                            ex.StudentTermRegisterID, 
                                            stu.SchoolID,
                                            tm.TermID ,
                                            tm.Year ,
                                            tm.TermNumber ,
                                            sub.SubjectCode,
                                            sub.SubjectName,
                                            stu.RegNumber,
                                            CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
                                            ex.Mark,
                                            ex.Grade,
                                            ex.Comment, 
                                            stu.IsMale ,
                                            tm.endDate AS EndOfTermDate 
                                    FROM Exam ex
                                    INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = ex.TeacherClassSubjectID
									INNER JOIN Term tm on tcs.TermID = tm.TermID
                                    INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
                                    INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ex.StudentTermRegisterID 
                                    INNER JOIN Student stu on stu.StudentID = enrol.StudentID 
                                    Where ex.TeacherClassSubjectID = @teacherClassSubjectID
                                    AND stu.SchoolID = @schoolID
                                    AND stu.IsDeleted IS NULL 
									AND tcs.IsDeleted IS NULL 
									AND ex.IsDeleted IS NULL 
									AND tm.IsDeleted IS NULL 
									AND sub.IsDeleted IS NULL 
									AND enrol.IsDeleted IS NULL  ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<ExamDto>(sqlTestMark,
                                new
                                {
                                    TeacherClassSubjectID = teacherClassSubjectID,
                                    schoolID = schoolID
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

        public List<ExamDto> GetListExamByStudentIdAndSubjectCode(Guid schoolID, Guid studentId, string subjectCode, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
                                            ex.TeacherClassSubjectID,
                                            ex.StudentTermRegisterID, 
                                            stu.SchoolID,
                                            tm.TermID ,
                                            tm.Year ,
                                            tm.TermNumber ,
                                            sub.SubjectCode,
                                            sub.SubjectName,
                                            stu.RegNumber,
                                            CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
                                            ex.Mark,
                                            ex.Grade,
                                            ex.Comment, 
                                            stu.IsMale ,
                                            tm.endDate AS EndOfTermDate 
                                    FROM Exam ex
                                    INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = ex.TeacherClassSubjectID
									INNER JOIN Term tm on tcs.TermID = tm.TermID
                                    INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
                                    INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ex.StudentTermRegisterID 
                                    INNER JOIN Student stu on stu.StudentID = enrol.StudentID 
                                    Where sub.SubjectCode = @subjectCode
                                    AND stu.SchoolID = @schoolID
                                    AND stu.StudentID = @studentID  
                                    AND stu.IsDeleted IS NULL 
									AND tcs.IsDeleted IS NULL 
									AND ex.IsDeleted IS NULL 
									AND tm.IsDeleted IS NULL 
									AND sub.IsDeleted IS NULL 
									AND enrol.IsDeleted IS NULL ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<ExamDto>(sqlTestMark,
                                new
                                {
                                    subjectCode = subjectCode,
                                    schoolID = schoolID,
                                    studentID = studentId
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

        public List<ExamDto> GetListExamByStudentId(Guid studentId, ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
                                            ex.TeacherClassSubjectID,
                                            ex.StudentTermRegisterID, 
                                            stu.SchoolID,
                                            tm.TermID ,
                                            tm.Year ,
                                            tm.TermNumber ,
                                            sub.SubjectCode,
                                            sub.SubjectName,
                                            stu.RegNumber,
                                            CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
                                            ex.Mark,
                                            ex.Grade,
                                            ex.Comment, 
                                            stu.IsMale ,
                                            tm.endDate AS EndOfTermDate 
                                    FROM Exam ex
                                    INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = ex.TeacherClassSubjectID
									INNER JOIN Term tm on tcs.TermID = tm.TermID
                                    INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
                                    INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ex.StudentTermRegisterID 
                                    INNER JOIN Student stu on stu.StudentID = enrol.StudentID 
                                    Where stu.StudentID = @studentID  
                                    AND stu.IsDeleted IS NULL 
									AND tcs.IsDeleted IS NULL 
									AND ex.IsDeleted IS NULL 
									AND tm.IsDeleted IS NULL 
									AND sub.IsDeleted IS NULL 
									AND enrol.IsDeleted IS NULL ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<ExamDto>(sqlTestMark,
                                new
                                {
                                    studentID = studentId
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

        public List<ExamDto> GetListExamByStudentTermRegisterIDAndTermID(Guid studentTermRegisterID, Guid termID ,ref bool dbError)
        {
            try
            {
                var sqlTestMark = @"SELECT  enrol.StudentID,
                                            ex.TeacherClassSubjectID,
                                            ex.StudentTermRegisterID, 
                                            stu.SchoolID,
                                            tm.TermID ,
                                            tm.Year ,
                                            tm.TermNumber ,
                                            sub.SubjectCode,
                                            sub.SubjectName,
                                            stu.RegNumber,
                                            CONCAT(stu.StudentName,'  ',stu.StudentSurname) As FullName,
                                            ex.Mark,
                                            ex.Grade,
                                            ex.Comment, 
                                            stu.IsMale ,
                                            tm.endDate AS EndOfTermDate 
                                    FROM Exam ex
                                    INNER JOIN TeacherClassSubject  tcs on tcs.TeacherClassSubjectID = ex.TeacherClassSubjectID
									INNER JOIN Term tm on tcs.TermID = tm.TermID
                                    INNER JOIN Subject sub on sub.SubjectID = tcs.SubjectID
                                    INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ex.StudentTermRegisterID 
                                    INNER JOIN Student stu on stu.StudentID = enrol.StudentID  
                                    Where enrol.TermID = @termID
                                    AND enrol.StudentTermRegisterID = @studentTermRegisterID 
                                    AND stu.IsDeleted IS NULL 
									AND tcs.IsDeleted IS NULL 
									AND ex.IsDeleted IS NULL 
									AND tm.IsDeleted IS NULL 
									AND sub.IsDeleted IS NULL 
									AND enrol.IsDeleted IS NULL 
                                    ";

                using (var connection = GetConnection())
                {
                    var list = connection.Query<ExamDto>(sqlTestMark,
                                new
                                {
                                    studentTermRegisterID = studentTermRegisterID,
                                    termID = termID
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

        
        public int CountAllExamWrittenByStudentAndTermID(Guid studentID, Guid termId , ref bool dbFlag)
        {
            var sql = @"SELECT COUNT(*)
                        FROM  Exam ex
                        INNER JOIN  StudentTermRegister enrol on enrol.StudentTermRegisterID = ex.StudentTermRegisterID 
                        AND enrol.StudentID = @studentID
                        AND enrol.TermID = @termId AND enrol.IsDeleted ISNULL AND ex.IsDeleted IS NULL";
            using (var connection = GetConnection())
            {
                var list = connection.Execute(sql,  new { studentID = studentID , termId = termId });
                return list;
            }
        }

        public bool Save(Exam exam,string modifiedBy , ref bool dbFlag)
        {
            using (var connection = GetConnection())
            {
                var update = @" 
                                UPDATE Exam
					                            SET  Mark = @Mark 
						                            ,Grade = @Grade 
						                            ,Comment = @Comment  
,LastModifiedBy = @modifiedBy
					                            WHERE StudentTermRegisterID = @StudentTermRegisterID
					                            AND TeacherClassSubjectID = @TeacherClassSubjectID 
AND IsDeleted IS NULL
                                ";
                var id = connection.Execute(update,
                             new
                             {
                                 StudentTermRegisterID = exam.StudentTermRegisterID,
                                 TeacherClassSubjectID = exam.TeacherClassSubjectID,
                                 Mark = exam.Mark,
                                 Grade = exam.Grade,
                                 Comment = exam.Comment
                             });
                if (id <= 0)
                {
                    var insert = @"
                                INSERT INTO Exam
                                   (StudentTermRegisterID
                                   ,TeacherClassSubjectID
                                   ,Mark
                                   ,Grade
                                   ,Comment , LAstModifiedBy)
                             VALUES
                                   (
                                    @StudentTermRegisterID 
                                   ,@TeacherClassSubjectID 
                                   ,@Mark 
                                   ,@Grade 
                                   ,@Comment  ,@modifiedBy
                                      ) 
                                ";
                    id = connection.Execute(insert,
                               new
                               {
                                   StudentTermRegisterID = exam.StudentTermRegisterID,
                                   TeacherClassSubjectID = exam.TeacherClassSubjectID,
                                   Mark = exam.Mark,
                                   Grade = exam.Grade,
                                   Comment = exam.Comment , 
                                   modifiedBy = modifiedBy
                               });


                }
                return true;
            }
        }
        public int Save(List<Exam> examList, string modifiedBy , ref bool dbFlag)
        {
            int rowSaved = 0;
            using (var connection = GetConnection())
            {
                foreach (var exam in examList)
                {
                    var update = @" 
                                UPDATE Exam
					                            SET  Mark = @Mark 
						                            ,Grade = @Grade 
						                            ,Comment = @Comment  
                                                   , LastModifiedBy  = @modifiedBy
					                            WHERE StudentTermRegisterID = @StudentTermRegisterID
					                            AND TeacherClassSubjectID = @TeacherClassSubjectID 
AND IsDeleted IS NULL
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     StudentTermRegisterID = exam.StudentTermRegisterID,
                                     TeacherClassSubjectID = exam.TeacherClassSubjectID,
                                     Mark = exam.Mark,
                                     Grade = exam.Grade,
                                     Comment = exam.Comment ,
                                     modifiedBy = modifiedBy
                                 });
                    if(id <= 0)
                    {
                        var insert = @"
                                INSERT INTO Exam
                                   (StudentTermRegisterID
                                   ,TeacherClassSubjectID
                                   ,Mark
                                   ,Grade
                                   ,Comment , LastModifiedBy )
                             VALUES
                                   (
                                    @StudentTermRegisterID 
                                   ,@TeacherClassSubjectID 
                                   ,@Mark 
                                   ,@Grade 
                                   ,@Comment  ,@modifiedBy
                                      ) 
                                ";
                          id = connection.Execute(insert,
                                     new
                                     {
                                         StudentTermRegisterID = exam.StudentTermRegisterID,
                                         TeacherClassSubjectID = exam.TeacherClassSubjectID,
                                         Mark = exam.Mark,
                                         Grade = exam.Grade,
                                         Comment = exam.Comment ,
                                         modifiedBy = modifiedBy
                                     });
                    }

                    rowSaved++;
                }
                

            }
            return rowSaved;
        }

        public bool Delete(Guid studentTermRegisterID, Guid teacherClassSubjectID, string modifiedBy , ref bool dbFlag)
        {
            using (var connection = GetConnection())
            {
                var update = @"UPDATE  Exam SET  isdeleted = now() , islive = null , LastModifiedBy = @modifiedBy
					                            WHERE StudentTermRegisterID = @StudentTermRegisterID
					                            AND TeacherClassSubjectID = @TeacherClassSubjectID AND IsDeleted IS NULL LIMIT 1;
                                ";
                var id = connection.Execute(update,
                             new
                             {
                                 StudentTermRegisterID = studentTermRegisterID,
                                 TeacherClassSubjectID = teacherClassSubjectID,
                                 modifiedBy = modifiedBy
                             });

                return true;
            }
        }
    }
}
