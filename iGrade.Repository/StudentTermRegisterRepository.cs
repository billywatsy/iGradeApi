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
    public class StudentTermRegisterRepository : BaseRepository
    {
        public List<StudentTermRegisterDto> GetLevelListByLevelIDAndSchoolIDAndTermID(Guid levelID, Guid schoolID, Guid termId, ref bool dbFlag)
        {
            var sql = @"SELECT
                           en.StudentTermRegisterID ,
                           en.StudentID ,
                           en.ClassID ,
                           cl.LevelID ,
                           en.TermID ,
                           st.DOB as DateOfBirth ,
                           st.RegNumber ,
                           CONCAT(st.StudentName,'  ',st.StudentSurname) as StudentName,
                           st.IDnational As IdNumber ,
                           cl.ClassName ,
                           lv.LevelName ,
                           tr.Year As TermYear,
                           tr.TermNumber ,
                           st.IsMale 
                          FROM StudentTermRegister en 
                          inner join Student st on en.StudentID = st.StudentID
                          inner join Class cl on en.ClassID = cl.ClassID 
                          inner join Level lv on lv.LevelID = cl.LevelID 
                          inner join Term tr on tr.TermID = en.TermID 
                          Where cl.SchoolID = @schoolID
                          AND cl.levelID = @levelID 
                          AND en.TermID = @termId";

            using (var connection = GetConnection())
            {
                var list = connection.Query<StudentTermRegisterDto>(sql,
                            new { levelID = levelID, schoolID = schoolID, termID = termId }
                                ).AsList();
                return list;
            }
        }

        public List<StudentTermRegisterDto> GetListByClassIDAndTermID(Guid classID, Guid termId, ref bool dbFlag)
        {
            var sql = @"SELECT
                           en.StudentTermRegisterID ,
                           en.StudentID ,
                           en.ClassID ,
                           cl.LevelID ,
                           en.TermID ,
                           st.DOB as DateOfBirth ,
                           st.RegNumber ,
                           CONCAT(st.StudentName,'  ',st.StudentSurname) as StudentName,
                           st.IDnational As IdNumber ,
                           cl.ClassName ,
                           lv.LevelName ,
                           tr.Year As TermYear,
                           tr.TermNumber ,
                           st.IsMale 
                          FROM StudentTermRegister en 
                          inner join Student st on en.StudentID = st.StudentID
                          inner join Class cl on en.ClassID = cl.ClassID
                          inner join Level lv on lv.LevelID = cl.LevelID
                          inner join Term tr on tr.TermID = en.TermID
                          Where en.ClassID = @classID
                          AND en.TermID = @termId
                          AND en.IsDELETED IS NULL
                          AND st.IsDELETED IS NULL
                          AND cl.IsDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<StudentTermRegisterDto>(sql, 
                            new { classID = classID , termId = termId } 
                                ).AsList();
                return list;
            }
        }
        
        public List<StudentTermRegisterDto> GetListAllByStudentID(Guid studentId, ref bool dbFlag)
        {
            var sql = @"SELECT
                           en.StudentTermRegisterID ,
                           en.StudentID ,
                           en.ClassID ,
                           cl.LevelID ,
                           en.TermID ,
                           st.DOB as DateOfBirth ,
                           st.RegNumber ,
                           CONCAT(st.StudentName,'  ',st.StudentSurname) as StudentName,
                           st.IDnational As IdNumber ,
                           cl.ClassName ,
                           lv.LevelName ,
                           tr.Year As TermYear,
                           tr.TermNumber ,
                           st.IsMale 
                          FROM StudentTermRegister en 
                          inner join Student st on en.StudentID = st.StudentID
                          inner join Class cl on en.ClassID = cl.ClassID 
                          inner join Level lv on lv.LevelID = cl.LevelID 
                          inner join Term tr on tr.TermID = en.TermID 
                          Where st.StudentID = @studentId
                          AND en.IsDELETED IS NULL
                          AND tr.IsDELETED IS NULL
                          AND st.IsDELETED IS NULL
                          AND cl.IsDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<StudentTermRegisterDto>(sql,
                            new { studentId = studentId}
                                ).AsList();
                return list;
            }
        }
        public List<StudentTermRegisterDto> GetClassListByClassIDAndSchoolIDAndTermID(Guid classID, Guid schoolID, Guid termId, ref bool dbFlag)
        {
            var sql = @"SELECT
                           en.StudentTermRegisterID ,
                           en.StudentID ,
                           en.ClassID ,
                           cl.LevelID ,
                           en.TermID ,
                           st.DOB as DateOfBirth ,
                           st.RegNumber ,
                           CONCAT(st.StudentName,'  ',st.StudentSurname) as StudentName,
                           st.IDnational As IdNumber ,
                           cl.ClassName ,
                           lv.LevelName ,
                           tr.Year As TermYear,
                           tr.TermNumber ,
                           st.IsMale 
                          FROM StudentTermRegister en 
                          inner join Student st on en.StudentID = st.StudentID
                          inner join Class cl on en.ClassID = cl.ClassID 
                          inner join Level lv on lv.LevelID = cl.LevelID 
                          inner join Term tr on tr.TermID = en.TermID 
                          Where cl.SchoolID = @schoolID
                          AND cl.ClassID = @classID 
                          AND en.TermID = @termId
                          AND en.IsDELETED IS NULL
                          AND tr.IsDELETED IS NULL
                          AND st.IsDELETED IS NULL
                          AND cl.IsDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<StudentTermRegisterDto>(sql,
                            new { classID = classID, schoolID = schoolID, termID = termId }
                                ).AsList();
                return list;
            }
        }

        public List<StudentTermRegisterDto> GetListBySchoolIDAndTermID(Guid schoolID, Guid termId, ref bool dbFlag)
        {
            var sql = @"SELECT
                           en.StudentTermRegisterID ,
                           en.StudentID ,
                           en.ClassID ,
                           cl.LevelID ,
                           en.TermID ,
                           st.DOB as DateOfBirth ,
                           st.RegNumber ,
                           CONCAT(st.StudentName,'  ',st.StudentSurname) as StudentName,
                           st.IDnational As IdNumber ,
                           cl.ClassName ,
                           lv.LevelName ,
                           tr.Year As TermYear,
                           tr.TermNumber ,
                           st.IsMale 
                          FROM StudentTermRegister en 
                          inner join Student st on en.StudentID = st.StudentID
                          inner join Class cl on en.ClassID = cl.ClassID 
                          inner join Level lv on lv.LevelID = cl.LevelID 
                          inner join Term tr on tr.TermID = en.TermID 
                          Where cl.SchoolID = @schoolID
                          AND en.TermID = @termId
                          AND en.IsDELETED IS NULL
                          AND tr.IsDELETED IS NULL
                          AND st.IsDELETED IS NULL
                          AND cl.IsDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<StudentTermRegisterDto>(sql, 
                            new { schoolID = schoolID, termID = termId } 
                                ).AsList();
                return list;
            }
        }
        public StudentTermRegister GetByStudentIDAndTermId(Guid studentID, Guid schoolID, Guid termId, ref bool dbFlag)
        {
            var sql = @"SELECT en.* , st.* ,cl.*
                          FROM StudentTermRegister en 
                          inner join Student st on en.StudentID = st.StudentID
                          inner join Class cl on en.ClassID = cl.ClassID
                          Where cl.SchoolID = @schoolID
                          AND en.StudentID = @studentID
                          AND en.TermID = @termId
                          AND en.IsDELETED IS NULL
                          AND st.IsDELETED IS NULL
                          AND cl.IsDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<StudentTermRegister, Student, Class, StudentTermRegister>(sql,
                            (enroll, stude, clas) =>
                            {
                                enroll.Student = stude;
                                enroll.Class = clas;
                                return enroll;
                            },
                            new { studentID = studentID, schoolID = schoolID, termId = termId }
                            , splitOn: "StudentID,ClassID"
                                ).FirstOrDefault();
                return list;
            }
        }


        public StudentTermRegisterDto GetByEnrollemtID(Guid studentTermRegisterID, ref bool dbFlag)
        {
            var sql = @"SELECT 
                            
                           en.StudentTermRegisterID ,
                           en.StudentID ,
                           en.ClassID ,
                           cl.LevelID ,
                           en.TermID ,
                           st.DOB as DateOfBirth ,
                           st.RegNumber ,
                           CONCAT(st.StudentName,'  ',st.StudentSurname) as StudentName,
                           st.IDnational As IdNumber ,
                           cl.ClassName ,
                           lv.LevelName ,
                           tr.Year As TermYear,
                           tr.TermNumber ,
                           st.IsMale  
                          FROM StudentTermRegister en 
                          inner join Student st on en.StudentID = st.StudentID
                          inner join Class cl on en.ClassID = cl.ClassID
                          inner join Level lv on lv.LevelID = cl.LevelID 
                          inner join Term tr on tr.TermID = en.TermID 
                          Where  en.StudentTermRegisterID = @studentTermRegisterID 
                          AND en.IsDELETED IS NULL
                          AND st.IsDELETED IS NULL
                          AND tr.IsDELETED IS NULL
                          AND cl.IsDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<StudentTermRegisterDto>(sql, new { studentTermRegisterID = studentTermRegisterID }).FirstOrDefault();
                return list;
            }
        }
 
        public StudentTermRegister GetByStudentIDAndTermId(Guid studentID, Guid termId, ref bool dbFlag)
        {
            var sql = @"SELECT  en.* , st.* ,cl.*
                          FROM StudentTermRegister en 
                          inner join Student st on en.StudentID = st.StudentID
                          inner join Class cl on en.ClassID = cl.ClassID
                          Where en.StudentID = @studentID
                          AND en.TermID = @termId 

                          AND en.IsDELETED IS NULL
                          AND st.IsDELETED IS NULL
                          AND cl.IsDELETED IS NULL LIMIT 1";

            using (var connection = GetConnection())
            {
                var list = connection.Query<StudentTermRegister, Student, Class, StudentTermRegister>(sql,
                            (enroll, stude, clas) =>
                            {
                                enroll.Student = stude;
                                enroll.Class = clas;
                                return enroll;
                            },
                            new { studentID = studentID , termId = termId }
                            , splitOn: "StudentID,ClassID"
                                ).FirstOrDefault();
                return list;
            }
        }

        public StudentTermRegister GetByID(Guid studentTermRegisterID,ref bool dbFlag)
        {
            var sql = @"SELECT en.* , st.* ,cl.*
                          FROM StudentTermRegister en 
                          inner join Student st on en.StudentID = st.StudentID
                          inner join Class cl on en.ClassID = cl.ClassID
                          Where en.StudentTermRegisterID = @studentTermRegisterID 
                          AND en.IsDELETED IS NULL
                          AND st.IsDELETED IS NULL
                          AND cl.IsDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<StudentTermRegister, Student, Class, StudentTermRegister>(sql,
                            (enroll, stude, clas) =>
                            {
                                enroll.Student = stude;
                                enroll.Class = clas;
                                return enroll;
                            },
                            new { studentTermRegisterID = studentTermRegisterID}
                            , splitOn: "StudentID,ClassID"
                                ).FirstOrDefault();
                return list;
            }
        }

        public StudentTermRegister IsStudentAlreadyEnrolledByStudentID(Guid studentID, Guid schoolID, Guid termId , ref bool dbFlag)
        {
            var sql = @"SELECT en.* , st.* ,cl.*
                          FROM StudentTermRegister en 
                          inner join Student st on en.StudentID = st.StudentID
                          inner join Class cl on en.ClassID = cl.ClassID
                          Where cl.SchoolID = @schoolID
                          AND en.StudentID = @studentID
                          AND en.TermID = @termId
                          AND en.IsDELETED IS NULL
                          AND st.IsDELETED IS NULL
                          AND cl.IsDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<StudentTermRegister, Student, Class, StudentTermRegister>(sql,
                            (enroll, stude, clas) =>
                            {
                                enroll.Student = stude;
                                enroll.Class = clas;
                                return enroll;
                            },
                            new { studentID = studentID, schoolID = schoolID }
                            , splitOn: "StudentID,ClassID"
                                ).FirstOrDefault();
                return list;
            }
        }
		public int BulkInsert(List<StudentTermRegister> studentTermRegisters, string modifiedBy ,ref bool dbError)
        {
            int savedRows = 0;
            try
            { 
                using (var connection = GetConnection())
				{
					foreach(var studentTermRegister in studentTermRegisters){
                        var ids = Guid.NewGuid();
						var sql = @"  
									INSERT INTO StudentTermRegister
									   (StudentTermRegisterID
									   ,StudentID
									   ,ClassID
									   ,TermID
									   ,IsPhoneSent
									   ,IsEmailSent
									   ,IsAllowedSent , LastModifiedBy)
									VALUES
									   (
										@id
									   ,@StudentID
									   ,@ClassID
									   ,@TermID
									   ,@IsPhoneSent
									   ,@IsEmailSent
									   ,@IsAllowedSent ,@modifiedBy
									   )
							
							";
							
							var id = connection.Execute(sql, new
							{ 
                                id = Guid.NewGuid() ,
								StudentID = studentTermRegister.StudentID,
								ClassID = studentTermRegister.ClassID,
								IsAllowedSent = studentTermRegister.IsAllowedSent,
								IsEmailSent = studentTermRegister.IsEmailSent,
								IsPhoneSent = studentTermRegister.IsPhoneSent,
								TermID = studentTermRegister.TermID ,
                                modifiedBy = modifiedBy
							});
							if(id >= 0)
							{
								savedRows++;
							}
						}
				}
                    
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
            }
			return savedRows;
        }
		
        public StudentTermRegister Insert(StudentTermRegister studentTermRegister, string modifiedBy ,ref bool dbError)
        {
            try
            { 
                var sql = @"   
									INSERT INTO StudentTermRegister
									   (StudentTermRegisterID
									   ,StudentID
									   ,ClassID
									   ,TermID
									   ,IsPhoneSent
									   ,IsEmailSent
									   ,IsAllowedSent
                                       , LastModifiedBy
                                       )
									VALUES
									   (
										@id
									   ,@StudentID
									   ,@ClassID
									   ,@TermID
									   ,@IsPhoneSent
									   ,@IsEmailSent
									   ,@IsAllowedSent
                                        ,@modifiedBy
									   )
							
							";
                studentTermRegister.StudentTermRegisterID = Guid.NewGuid();
                    var id = GetConnection().Execute(sql, new
                    { 
                        id = studentTermRegister.StudentTermRegisterID,
                        StudentID = studentTermRegister.StudentID,
                        ClassID = studentTermRegister.ClassID,
                        IsAllowedSent = studentTermRegister.IsAllowedSent,
                        IsEmailSent = studentTermRegister.IsEmailSent,
                        IsPhoneSent = studentTermRegister.IsPhoneSent,
                        TermID = studentTermRegister.TermID ,
                        modifiedBy = modifiedBy
                    });
					if(id <= 0)
					{
                     return null;
					}
                    return studentTermRegister;
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return null;
            }
        }
        public StudentTermRegister Update(StudentTermRegister studentTermRegister, string modifiedBy , ref bool dbError)
        {
            try
            {
                var sql = @"UPDATE StudentTermRegister
                               SET IsPhoneSent = @IsPhoneSent
                                  ,IsEmailSent = @IsEmailSent
                                  ,IsAllowedSent = @IsAllowedSent
                                  ,CanParentViewExamData = @CanParentViewExamData
                                  ,LastModifiedBy = @modifiedBy
                            WHERE StudentTermRegisterID = @StudentTermRegisterID AND IsDeleted IS NULL";
                
                    var id = GetConnection().Execute(sql, new
                    {
                        StudentTermRegisterID = studentTermRegister.StudentTermRegisterID,
                        IsAllowedSent = studentTermRegister.IsAllowedSent,
                        IsEmailSent = studentTermRegister.IsEmailSent,
                        IsPhoneSent = studentTermRegister.IsPhoneSent,
                        TermID = studentTermRegister.TermID ,
                        CanParentViewExamData = studentTermRegister.CanParentViewExamData ,
                        modifiedBy = modifiedBy
                    });
                return studentTermRegister;
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return null;
            }
        }

        public bool DeleteByID(Guid studentTermRegisterId ,string modifiedBy , ref bool dbError)
        {
            try
            {
                // remove exam , test based on studentTermRegisterId
                var sql = "UPDATE StudentTermRegister SET  isdeleted = now() , islive = null , LASTMODIFIEDBY = @modifiedBy Where StudentTermRegisterID = @StudentTermRegisterID ";
                
                    var id = GetConnection().Execute(sql, new
                    {
                        StudentTermRegisterID = studentTermRegisterId,
                        modifiedBy = modifiedBy
                    });
                return true;
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
