using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace iGrade.Repository
{
    public class TeacherRepository : BaseRepository
    {
        public Teacher Save(Teacher teacher, string onetimePin ,string modifiedby ,  ref bool dbError)
        {
            try
            {
                if (teacher.TeacherID == null || Guid.Empty == teacher.TeacherID)
                {
                    onetimePin = HashPassword(onetimePin);
                    var id = Guid.NewGuid();
                    var forgotCode = Guid.NewGuid().ToString().Substring(0,3);
                    var sql = @"
INSERT INTO Teacher
           (TeacherID
           ,SchoolID
           ,IsAdmin
           ,TeacherUsername
           ,TeacherEmail
           ,TeacherFullname
           ,TeacherPhone
           ,IsActive
           ,CreatedDate
           ,LastModifiedBy)
     VALUES
           (
            @id
           ,@SchoolID
           ,@IsAdmin 
           ,@TeacherUsername
           ,@TeacherEmail
           ,@TeacherFullname
           ,@TeacherPhone
           ,@IsActive 
           ,@CreatedDate 
           ,@modifiedby );

INSERT INTO Auth
           (TeacherID
           ,Password
           ,OneTimePin
           ,ForgotPassowrdCode
           ,isExpired
           ,CreatedDate , 
            WebToken , LastModifiedBy)
     VALUES
           (@id
           ,@oneTimePin
           ,@oneTimePin
           ,@forgotCode
           ,1
           ,@createdDate, @token , @modifiedby);

";
                    var idr = GetConnection().Execute(sql, new {
                        id = id ,
                        token = Guid.NewGuid() ,
                        TeacherID = teacher.TeacherID ,
                        SchoolID= teacher.SchoolID ,
                        IsAdmin = teacher.IsAdmin ,
                        oneTimePin = onetimePin ,
                        forgotCode = forgotCode, 
                        TeacherUsername = teacher.TeacherUsername ,
                        TeacherEmail = teacher.TeacherEmail  ,
                        TeacherFullname = teacher.TeacherFullname    ,
                        TeacherPhone = teacher.TeacherPhone ,
                        IsActive = teacher.IsActive  ,
                        CreatedDate = DateTime.Today ,
                        modifiedby = @modifiedby
                    }) ;
                    teacher.TeacherID = id;
                }
                else
                {
                    var sqlUdate = @"UPDATE Teacher SET 
                                    IsAdmin = @IsAdmin , 
                                    TeacherEmail = @TeacherEmail  ,
                                    TeacherFullname = @TeacherFullname    ,
                                    TeacherPhone = @TeacherPhone ,
                                    IsActive = @IsActive  ,
                                    Lastmodifiedby = @modifiedby
                                    WHERE TeacherID = @TeacherID 
                                    
                                    ";

                    var id = GetConnection().Execute(sqlUdate, new
                            {
                                TeacherID = teacher.TeacherID, 
                                IsAdmin = teacher.IsAdmin,
                                TeacherUsername = teacher.TeacherUsername,
                                TeacherEmail = teacher.TeacherEmail,
                                TeacherFullname = teacher.TeacherFullname,
                                TeacherPhone = teacher.TeacherPhone,
                                IsActive = teacher.IsActive  , 
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
            return teacher;
        }

        public Teacher GetTeacherById(Guid teacherID, ref bool dbError)
        {
            try
            {
                var sql = @"SELECT   tec.* 
                              FROM Teacher tec 
                              inner join School sch on sch.SchoolID = tec.SchoolID
                              where tec.TeacherId = @teacherID AND tec.IsDELETED IS NULL AND sch.ISDELETED IS NULL";

                using (var connection = GetConnection())
                {
                    var record = connection.Query<Teacher>(sql,
                                  new { teacherID = teacherID }
                                    ).FirstOrDefault();
                    return record;
                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
            }
            return null;
        }


        public bool IsEmailExist(string email, ref bool dbError)
        {
            try
            {
                var sql = @"SELECT   tec.*  
                              FROM Teacher tec  
                              where tec.TeacherEmail = @email ";

                using (var connection = GetConnection())
                {
                    var record = connection.Query<Teacher>(sql,
                                  new { email = email }
                                    ).FirstOrDefault();
                    if (record != null)
                    {
                        return true;
                    }
                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
            }
            return false;
        }

        public bool IsUsernameExist(string username, ref bool dbError)
        {
            try
            {
                var sql = @"SELECT   tec.*  
                              FROM Teacher tec  
                              where tec.TeacherUsername = @username ";

                using (var connection = GetConnection())
                {
                    var record = connection.Query<Teacher>(sql,
                                  new { username = username }
                                    ).FirstOrDefault();
                    if (record != null)
                    {
                        return true;
                    }
                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
            }
            return false;
        }

        public bool IsPhoneExist(string phone, ref bool dbError)
        {
            try
            {
                var sql = @"SELECT   tec.*  
                              FROM Teacher tec  
                              where tec.TeacherPhone = @phone ";

                using (var connection = GetConnection())
                {
                    var record = connection.Query<Teacher>(sql,
                                  new { phone = phone }
                                    ).FirstOrDefault();
                    if (record != null)
                    {
                        return true;
                    }
                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
            }
            return false;
        }




        public Teacher GetTeacherByEmail(string email, ref bool dbError)
        {
            try
            {
                var sql = @"SELECT   tec.*  
                              FROM Teacher tec  
                              where tec.TeacherEmail = @email AND tec.ISDELETED IS NULL ";

                using (var connection = GetConnection())
                {
                    var record = connection.Query<Teacher>(sql, 
                                  new { email = email } 
                                    ).FirstOrDefault();
                    return record;
                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
            }
            return null;
        }

        public List<Teacher> GetListTeacherBySchoolId(Guid schoolId, ref bool dbError)
        {
            try
            {
                var sql = @"SELECT    tec.* 
                              FROM Teacher tec
                              inner join School sch on sch.SchoolID = tec.SchoolID
                              where tec.SchoolID = @schoolId  AND tec.ISDELETED IS NULL  AND sch.ISDELETED IS NULL  ";

                var parameters = new DynamicParameters();
                parameters.Add("@schoolID", schoolId);
                return GetList<Teacher>(sql, parameters);
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
            }
            return null;
        }

        public List<Teacher> GetListTeacherBySchoolIdAndActiveStatus(Guid schoolId, bool IsActive ,ref bool dbError)
        {
            try
            {
                var sql = @"SELECT    tec.* 
                              FROM Teacher tec
                              inner join School sch on sch.SchoolID = tec.SchoolID
                              where tec.SchoolID = @schoolId
                              AND tec.IsActive = @IsActive  AND sch.ISDELETED IS NULL  AND tec.ISDELETED IS NULL ";

                var parameters = new DynamicParameters();
                parameters.Add("@schoolID", schoolId );
                parameters.Add("@IsActive", IsActive);
                return GetList<Teacher>(sql, parameters);
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
            }
            return null;
        }

        public int CountTeachersBySchoolId(Guid schoolId, ref bool dbError)
        {
            try
            {
                var sql = @"SELECT Count(*)
                              FROM Teacher tec 
                              inner join School sch on sch.SchoolID = tec.SchoolID
                              where tec.SchoolID = @schoolId  AND sch.ISDELETED IS NULL  AND tec.ISDELETED IS NULL ";

                using (var connection = GetConnection())
                {
                    return connection.ExecuteScalar<int>(sql, new { schoolID = schoolId });
                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
            }
            return 0;
        }

        public bool Delete(Guid teacherId, string modifiedby , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @"  UPDATE  Teacher SET  isdeleted = now() , islive = null  , LAstModifiedby = @modifiedby   WHERE TeacherId = @teacherId
                            AND TeacherID NOT IN ( SELECT TeacherID FROM TeacherClassSubject Where TeacherID = @teacherId AND ISDELETED IS NULL )";
                    var id = connection.Execute(update, new
                    {
                        teacherId = teacherId,
                        modifiedby = modifiedby
                    });

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

        public PagedList<TeacherDto> GetPagedSearch(Guid schoolID, int PageSize, int PageNo, string searchValue, ref bool dbError)
        {
            try
            {
                var columnName = "TeacherFullname";

                if (PageNo <= 0)
                {
                    PageNo = 1;
                }
                var results = new PagedList<TeacherDto>();
                var sql = string.Format(@"SELECT *
							FROM Teacher
							WHERE  SchoolID = @schoolID 
                            AND ISDELETED IS NULL
							AND {0} LIKE @searchValue
							ORDER BY {1}
                            LIMIT @PageSize  OFFSET @Offset ;
							
							SELECT Count(*)
							FROM Teacher
							WHERE  SchoolID = @schoolID
                            AND ISDELETED IS NULL 
							AND {2} LIKE @searchValue
							", columnName, columnName, columnName);
                using (var connection = GetConnection())
                {
                    using (var multi = connection.QueryMultiple(sql
                                ,
                                new
                                {
                                    schoolID = schoolID,
                                    searchValue = "%" + searchValue + "%",
                                    PageSize = PageSize,
                                    PageNo = PageNo ,
                                    OffSet = (PageNo - 1) * PageSize
                                }))
                    {
                        results.PagedData = multi.Read<TeacherDto>().ToList();
                        results.TotalCount = multi.Read<int>().FirstOrDefault();
                        results.Page = PageNo;
                        results.Size = PageSize;
                        return results;
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
