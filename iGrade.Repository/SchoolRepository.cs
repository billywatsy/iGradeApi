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
    public class SchoolRepository : BaseRepository
    {
        public School GetSchoolByCode(string code, ref bool dbFlag)
        {
            var sql = @"SELECT   sc.* 
                         FROM  School sc 
                         WHERE SchoolCode = @code AND IsDeleted IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<School>(sql 
                            , new { code = code } 
                                ).FirstOrDefault();
                return list;
            }
        }
        public School GetSchoolBySchoolID(Guid schoolID, ref bool dbFlag)
        {
            var sql = @"SELECT   sc.*  
                         FROM  School sc 
                         WHERE SchoolID = @schoolID  AND IsDeleted IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<School>(sql, new { schoolID = schoolID }  ).FirstOrDefault();
                return list;
            }
        }

        public List<School> GetList(ref bool dbFlag)
        {
            var sql = @" SELECT * FROM  School WHERE IsDeleted IS NULL ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<School>(sql).AsList();
                return list;
            }
        }

        public bool SchoolActivateOrDeactivateAccount(Guid schoolID, bool isActive)
        {
            var sql = @"UPDATE SCHOOL SET IsActive = @isActive WHERE SchoolID = @schoolID  AND IsDeleted IS NULL ";
            using (var connection = GetConnection())
            {
                var id = connection.Execute(sql,
                            new
                            {
                                isActive = isActive ,
                                schoolID = schoolID
                            });

                return true;
            }
        }

        public object List()
        {
            var sql = @"SEl";
            return "";
        }

        public bool CreateNewSchool(School school , Term term , Setting setting , Teacher teacher ,string password , ref bool dbError)
        {
            password = HashPassword(password);

            var termID = Guid.NewGuid();
            var teacherid = Guid.NewGuid();
            var id = Guid.NewGuid();
            var webtoken = Guid.NewGuid();
            var sql = @"
                        INSERT INTO School
                           (SchoolID
                           ,SchoolName
                           ,SchoolCode
                           ,IsSchoolActive
                            )
                        VALUES
                           (@id
                           ,@name
                           ,@scode
                           ,1 
		                   );

		               INSERT INTO Term
                       (TermID
                       ,SchoolID
                       ,Year
                       ,TermNumber
                       ,FromDate
                       ,ToDate)
                 VALUES
                       (@termid
                       ,@id
                       ,@year
                       ,@termNumber
                       ,@fromTerm
                       ,@toTerm
		               );

                INSERT INTO Setting
                           (SchoolID
                           ,TermID
                           ,ExamMarkSubmissionClosingDate
                           ,TestMarkSubmissionClosingDate 
                           ,TermLastDateUpdated
                           ,MaximumTermPerYear )
                     VALUES
                           (@id
                           ,@termid
                           ,@dateToday
                           ,@dateToday
                           ,@dateToday
                           ,@numberOfTerm 
		                   );

                INSERT INTO Teacher
                           (TeacherID
                           ,SchoolID
                           ,IsAdmin
                           ,TeacherUsername
                           ,TeacherEmail
                           ,TeacherFullname
                           ,TeacherPhone
                           ,IsActive
                           ,CreatedDate)
                     VALUES
                           (
                            @teacherid
                           ,@id
                           ,1 
                           ,@TeacherUsername
                           ,@TeacherEmail
                           ,'School Admin'
                           ,@TeacherPhone
                           ,1
                           ,@dateToday );

                INSERT INTO Auth
                           (TeacherID
                           ,Password
                           ,ForgotPassowrdCode
                           ,isExpired
                           ,CreatedDate
                           ,WebToken)
                     VALUES
                           (@teacherid
                           ,@password
                           ,@id
                           ,1
                           ,@dateToday
                            ,@webtoken); ";
            using (var connection = GetConnection())
            {
                var ids = connection.Execute(sql,
                            new
                            {
                                id = id , 
                                termID = termID , 
                                teacherid = teacherid ,
                                dateToday = DateTime.Today ,
                                name = school.SchoolName ,
                                scode = school.SchoolCode , 
                                year = term.Year , 
                                termNumber = term.TermNumber , 
                                fromTerm = term.StartDate , 
                                toTerm = term.EndDate ,
                                numberOfTerm = setting.MaximumTermPerYear , 
                                TeacherUsername = teacher.TeacherUsername ,
                                TeacherEmail = teacher.TeacherEmail ,
                                TeacherPhone = teacher.TeacherPhone  ,
                                password = password ,
                                webtoken = webtoken
                            });

                return true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schoolId">if is nulll then get all</param>
        /// <param name="termYear"></param>
        /// <param name="termNumber"></param>
        /// <param name="dbFlag"></param>
        /// <returns></returns>
        public List<SchoolTermYearDataDto> GetAllSchoolsByTermAndYear(Guid? schoolId, int termYear, int termNumber, ref bool dbFlag)
        {
            var sql = @"SELECT sch.SchoolID , 
sch.SchoolName ,
(
 SELECT COUNT(DISTINCT(enrollment.EnrollmentID)) FROM enrollment Where  IsDeleted IS NULL AND enrollment.StudentID IN
    ( 
        SELECT student.StudentID FROM student Where student.SchoolID = sch.SchoolID  AND IsDeleted IS NULL
    )
    AND enrollment.TermID = (
        						SELECT term.TermID FROM TERM WHERE term.TermNumber = @termNumber 
        					   AND term.Year = @termYear
        						AND term.SchoolID = sch.SchoolID
 AND IsDeleted IS NULL
    						)
) As NumberOfStudents , 
(
 SELECT COUNT(DISTINCT(teacherclasssubject.TeacherID)) FROM teacherclasssubject Where teacherclasssubject.TeacherID IN
    ( 
        SELECT teacher.TeacherID FROM teacher Where teacher.SchoolID = sch.SchoolID  AND IsDeleted IS NULL
    )
 AND IsDeleted IS NULL
    AND teacherclasssubject.TermID = (
        						SELECT term.TermID FROM TERM WHERE term.TermNumber = @termNumber 
        					   AND term.Year = @termYear
        						AND term.SchoolID = sch.SchoolID  AND IsDeleted IS NULL
    						)
) As NumberOfTeachers , 
(
  SELECT COUNT(absentfromschool.EnrollmentID ) FROM absentfromschool  
  INNER JOIN enrollment on enrollment.EnrollmentID = absentfromschool.EnrollmentID
  Where enrollment.StudentID
    IN
    ( 
        SELECT student.StudentID FROM student Where student.SchoolID = sch.SchoolID  AND IsDeleted IS NULL
    )
    AND enrollment.TermID = (
        						SELECT term.TermID FROM TERM WHERE term.TermNumber = @termNumber 
        					   AND term.Year = @termYear
        						AND term.SchoolID = sch.SchoolID
 AND absentfromschool.IsDeleted IS NULL
 AND enrollment.IsDeleted IS NULL
    						)
) As TotalNumberOfAbsents  , 
(
  SELECT COUNT(class.ClassID) FROM class WHERE class.SchoolID = sch.SchoolID  AND IsDeleted IS NULL
) As NumberOfClass 
FROM school sch  AND sch.IsDeleted IS NULL";
            if (schoolId == null)
            {
                using (var connection = GetConnection())
                {
                    try
                    {
                        var list = connection.Query<SchoolTermYearDataDto>(sql, new { termNumber = termNumber, termYear = termYear }).AsList();
                        return list;
                    }
                    catch (Exception er)
                    {
                        return null;
                    }
                }
            }
            else
            {
                sql += " WHERE sch.SchoolID = @schoolId";
                using (var connection = GetConnection())
                {
                    try
                    {
                        var list = connection.Query<SchoolTermYearDataDto>(sql, new { termNumber = termNumber, termYear = termYear, schoolId = schoolId }).AsList();
                        return list;
                    }
                    catch (Exception er)
                    {
                        return null;
                    }
                }
            }
        }

    }
}
