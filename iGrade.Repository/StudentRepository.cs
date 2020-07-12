using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper; 

namespace iGrade.Repository
{
    public class StudentRepository : BaseRepository
    { 

        public bool Delete(Guid studentID,string modifiedby , ref bool dbError)
        {
            try
            {
                
                    var sql = @" UPDATE Student SET lastmodifiedby = @modifiedby , isdeleted = now() , islive = null  WHERE StudentID = @studentID  AND ISDeleted IS NULL
                          AND StudentID NOT IN (Select StudentID FROM Enrollment WHERE IsDeleted IS NULL AND StudentID = @studentID  )
                    ";
                    var id = GetConnection().Execute(sql, new
                    {
                        StudentID = studentID ,
                        modifiedby = modifiedby
                    });
                    if (id > 0)
                    {
                        return true;
                    }
                    return true;
                
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
            }
            return false;
        }

        public Student Save(Student student, string modifiedby ,ref bool dbError)
        {
            try
            {
                if (student.StudentID == null)
                {
                    student.RegNumber = CleanIDcodeAlphanumeric(student.RegNumber);
                    student.IDnational = CleanIDcodeAlphanumeric(student.IDnational);
                    student.StudentDateCreated = DateTime.Now;
                    var id = Guid.NewGuid();
                    var sql = @"
    INSERT INTO Student
           (StudentID
           ,SchoolID
           ,StudentName
           ,StudentMidName
           ,StudentSurname
           ,RegNumber
           ,IDnational
           ,IsMale
           ,DOB
           ,Email
           ,Phone
           ,StudentDateCreated , lastmodifiedby)
     VALUES
           (
            @id, 
           @SchoolID, 
           @StudentName, 
           @StudentMidName, 
           @StudentSurname, 
           @RegNumber, 
           @IDnational, 
           @IsMale,
           @DOB, 
           @Email, 
           @Phone, 
           @StudentDateCreated ,@modifiedby
            );
        ";
                    var tru = GetConnection().Query<Guid>(sql, new
                    {
                        id = id,
                        SchoolID = student.SchoolID,
                        StudentName = student.StudentName,
                        StudentMidName = student.StudentMidName,
                        StudentSurname = student.StudentSurname,
                        RegNumber = student.RegNumber,
                        IDnational = student.IDnational,
                        IsMale = student.IsMale,
                        DOB = student.DOB,
                        Email = student.Email,
                        Phone = student.Phone,
                        StudentDateCreated = DateTime.Today ,
                        modifiedby = @modifiedby
                    }).FirstOrDefault(); 
                
                    if(id != null)
                    {
                        student.StudentID = id;
                    }
                }
                else
                {
                    var sql = @"UPDATE   Student SET 
                                    StudentName = @StudentName ,
                                    StudentMidName = @StudentMidName ,
                                    StudentSurname = @StudentSurname ,
                                    IsMale = @IsMale ,
                                    DOB = @DOB ,
                                    Email = @Email ,
                                    Phone = @Phone ,
                                    LastModifiedBy = @modifiedby
                                WHERE StudentID = @StudentID  limit 1
                                ";
                    var id = GetConnection().Execute(sql, new
                    {
                        StudentID = student.StudentID, 
                        StudentName = student.StudentName,
                        StudentMidName = student.StudentMidName,
                        StudentSurname = student.StudentSurname,  
                        IsMale = student.IsMale,
                        DOB = student.DOB,
                        Email = student.Email,
                        Phone = student.Phone ,
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
            return student;
        }

        public Student GetStudentById(Guid studentID, ref bool dbError)
        {
            try
            {
                var sql = @"SELECT   st.*
                            FROM  Student st
                            INNER JOIN School sc on sc.SchoolID = st.SchoolID 
                            AND st.StudentID = @studentID 
                            AND st.ISDELETED IS NULL 
                            AND sc.ISDELETED IS NULL ";
                using (var connection = GetConnection())
                {
                    var list = connection.Query<Student>(sql , new { studentID = studentID }).FirstOrDefault();
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
        public List<Student> GetListOfStudentsLinkedToEmailAndSchoolID(string email, Guid schoolID, ref bool dbError)
        {
            try
            {
                var sql = @"SELECT st.*
                            FROM  Student st
                            INNER JOIN School sc on sc.SchoolID = st.SchoolID 
                            Where st.SchoolID = @schoolID 
                            And st.Email = @email 
                            AND st.ISDELETED IS NULL 
                            AND sc.ISDELETED IS NULL";
                using (var connection = GetConnection())
                {
                    var list = connection.Query<Student>(sql,
                                new
                                {
                                    schoolID = schoolID,
                                    email = email
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

        public List<Student> GetListSearchStudentByRegNumberAndSchoolID(string regNumber, Guid schoolID, ref bool dbError)
        {
            try
            {
                var sql = @"SELECT st.*
                            FROM  Student st
                            INNER JOIN School sc on sc.SchoolID = st.SchoolID 
                            Where st.SchoolID = @schoolID 
                            And st.RegNumber = @regNumber 
                            AND st.ISDELETED IS NULL 
                            AND sc.ISDELETED IS NULL";
                using (var connection = GetConnection())
                {
                    var list = connection.Query<Student>(sql,
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

        public List<Student> GetIsExistListStudentInRegNumberOrNationalID(List<string> ids, Guid schoolID,bool isRegNumber , ref bool dbError)
        {
            try
            {
                if(ids == null)
                {
                    return null;
                }
                string[] idsString =  ids.ToArray();
                string sql = @"SELECT st.*
                            FROM  Student st
                            INNER JOIN School sc on sc.SchoolID = st.SchoolID 
                            Where st.SchoolID = @schoolID 
                            And st.IDnational IN @idsString 
                            AND st.ISDELETED IS NULL 
                            AND sc.ISDELETED IS NULL";
                if (isRegNumber)
                {
                    sql = @"SELECT st.*
                            FROM  Student st
                            INNER JOIN School sc on sc.SchoolID = st.SchoolID 
                            Where st.SchoolID = @schoolID 
                            AND st.ISDELETED IS NULL 
                            AND sc.ISDELETED IS NULL
                            And st.RegNumber IN @idsString";
                }
                
                using (var connection = GetConnection())
                {
                    var list = connection.Query<Student>(sql,
                                new
                                {
                                    idsString = idsString,
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
        public Student GetStudentByRegNumber(string regNumber, Guid schoolID, ref bool dbError)
        {
            try
            {
                var sql = @"SELECT   st.*
                            FROM  Student st
                            INNER JOIN School sc on sc.SchoolID = st.SchoolID 
                            AND st.RegNumber = @regNumber
                            AND st.SchoolID = @schoolID 
                            AND st.ISDELETED IS NULL 
                            AND sc.ISDELETED IS NULL";
                using (var connection = GetConnection())
                {
                    var list = connection.Query<Student>(sql,
                                new
                                {
                                    RegNumber = regNumber,
                                    schoolID = schoolID
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

        public Student GetStudentByRegNumberAndSchoolCode(string regNumber, string schoolCode, ref bool dbError)
        {
            try
            {
                var sql = @"SELECT   st.*
                            FROM  Student st
                            INNER JOIN School sc on sc.SchoolID = st.SchoolID 
                            AND st.RegNumber = @regNumber
                            AND st.SchoolID IN (SELECT schoolID FROM School WHERE SchoolCode = @schoolCode ) 
                            AND st.ISDELETED IS NULL 
                            AND sc.ISDELETED IS NULL";
                using (var connection = GetConnection())
                {
                    var list = connection.Query<Student>(sql,
                                new
                                {
                                    RegNumber = regNumber,
                                    schoolCode = schoolCode
                                } ).FirstOrDefault();
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

        public Student GetStudentByNationalID(string iDnational, Guid schoolID, ref bool dbError)
        {
            try
            {
                var sql = @"SELECT st.*
                            FROM  Student st
                            INNER JOIN School sc on sc.SchoolID = st.SchoolID 
                            AND st.IDnational = @studentID
                            AND st.SchoolID = @schoolID 
                            AND st.ISDELETED IS NULL 
                            AND sc.ISDELETED IS NULL";
                using (var connection = GetConnection())
                {
                    var list = connection.Query<Student>(sql
                                , 
                                new { IDnational = iDnational , 
                                      schoolID = schoolID
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

        public int GetNumberOfStudentsCreatedInSchoolByYear(Guid schoolID, DateTime dateCreated, ref bool dbFlag)
        {
            var sql = @"SELECT Count(*)
                          FROM Student 
                          where SchoolID = @schoolID
                          And year(StudentDateCreated) = year(@dateCreated) 
                            AND ISDELETED IS NULL ";

            using (var connection = GetConnection())
            {
                return connection.ExecuteScalar<int>(sql, new { schoolID = schoolID , dateCreated = dateCreated });
            }
        }
        
    
	     public PagedList<Student> GetPagedStudentSearch(Guid schoolID , int PageSize , int PageNo , string searchValue, ref bool dbError)
		 { 			
			 try
            {
                var columnName = "StudentName";

                if (PageNo <= 0)
				{
					PageNo = 1;
				}
                var results = new PagedList<Student>();
                var sql = string.Format(@"SELECT *
							FROM Student
							WHERE  SchoolID = @schoolID 
							AND (
                            {0} LIKE @searchValue
                            OR RegNumber LIKE @searchValue
                            OR StudentSurname LIKE @searchValue
                            OR IDnational LIKE @searchValue
                            )
                            AND ISDELETED IS NULL 
                            AND ISDELETED IS NULL
							ORDER BY {1}
                            LIMIT @PageSize  OFFSET @Offset ;
							
							SELECT Count(*)
							FROM Student
							WHERE  SchoolID = @schoolID 
                            AND (
                             RegNumber LIKE @searchValue
                            OR StudentSurname LIKE @searchValue
                            OR IDnational LIKE @searchValue
							AND {2} LIKE @searchValue
                            )
                            AND ISDELETED IS NULL 
							", columnName, columnName, columnName);
                if (string.IsNullOrEmpty(searchValue))
                {
                    sql = string.Format(@"SELECT *
							FROM Student
							WHERE  SchoolID = @schoolID  
                            AND ISDELETED IS NULL 
							ORDER BY {0}
                            LIMIT @PageSize  OFFSET @Offset ;
							
							SELECT Count(*)
							FROM Student
							WHERE  SchoolID = @schoolID 
                            AND ISDELETED IS NULL
							", columnName);
                    using (IDbConnection connection = GetConnection())
                    {
                        using (var multi = connection.QueryMultiple(sql
                                    ,
                                    new
                                    {
                                        schoolID = schoolID,
                                        PageSize = PageSize,
                                        OffSet  =  (PageNo - 1) * PageSize
                                    }))
                        {
                            results.PagedData = multi.Read<Student>().ToList();
                            results.TotalCount = multi.Read<int>().FirstOrDefault();
                            results.Page = PageNo;
                            results.Size = PageSize;
                            return results;
                        }

                    }
                }
                else
                {
                    using (IDbConnection connection = GetConnection())
                    {
                        using (var multi = connection.QueryMultiple(sql
                                    ,
                                    new
                                    {
                                        schoolID = schoolID,
                                        searchValue = "%" + searchValue + "%",
                                        PageSize = PageSize,
                                        OffSet = (PageNo - 1) * PageSize
                                    }))
                        {
                            results.PagedData = multi.Read<Student>().ToList();
                            results.TotalCount = multi.Read<int>().FirstOrDefault();
                            results.Page = PageNo;
                            results.Size = PageNo;
                            return results;
                        }

                    }
                }
                
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
            }
            return null;
		 }
	}
}
