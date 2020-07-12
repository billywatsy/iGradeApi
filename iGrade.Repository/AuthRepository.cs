using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using iGrade.Domain;

namespace iGrade.Repository
{
    public class AuthRepository : BaseRepository
    { 

        public bool SystemAdminLogIn(string username , string password , ref bool dbflag)
        {
            string isLoggedIn = null;

            var sql = @"Select Username
                        FROM Admin 
                        WHERE Username = @username
                        AND Password = @password
                         AND IsDeleted Is NULL";

            using (var connection = GetConnection())
            {
                isLoggedIn = connection.Query<string>(sql, new { username = username, password = password }).FirstOrDefault();
            }
            if (string.IsNullOrEmpty(isLoggedIn))
            {
                return false;
            }
            return true;
        }

        public Teacher Login(string username, string password, ref bool dbFlag)
        {
            Teacher isLoggedIn = null;
            password = HashPassword(password);
            var sql = @"Select Teacher.*
                        FROM Teacher
                        INNER JOIN Auth on Auth.TeacherID = Teacher.TeacherID
                        WHERE Teacher.TeacherUsername = @username
                        AND Auth.Password = @password  AND Auth.IsDeleted Is NULL  AND Teacher.IsDeleted Is NULL";

            using (var connection = GetConnection())
            {
                isLoggedIn = connection.Query<Teacher>(sql, new { username = username, password = password }).FirstOrDefault();
            }
            return isLoggedIn;
        }

        public Teacher LoginOneTimePin(string username, string onetimepin, ref bool dbFlag)
        {
            Teacher isLoggedIn = null;
            onetimepin = HashPassword(onetimepin);
            var sql = @"Select Teacher.*
                        FROM Teacher
                        INNER JOIN Auth on Auth.TeacherID = Teacher.TeacherID
                        WHERE Teacher.TeacherUsername = @username
                        AND Year(Auth.LastLogin) = Year(now()) 
                        AND Month(Auth.LastLogin) = Month(now()) 
                        AND Day(Auth.LastLogin) = Day(now()) 
                        AND Auth.OneTimePin = @onetimepin  AND Auth.IsDeleted Is NULL  AND Teacher.IsDeleted Is NULL";

            using (var connection = GetConnection())
            {
                isLoggedIn = connection.Query<Teacher>(sql, new { username = username, onetimepin = onetimepin }).FirstOrDefault();
            }
            return isLoggedIn;
        }

        public iGrade.Domain.Dto.LoggedUser SessionSetting(Guid teacherID, ref bool dbFlag)
        {

            var sql = @"Select  tea.TeacherID 
		                ,
		                (
			                SELECT  ClassTeacher.ClassID
			                FROM ClassTeacher 
			                INNER JOIN Class on Class.ClassID = ClassTeacher.ClassID
			                INNER JOIN Setting on Setting.TermID = ClassTeacher.TermID
			                WHERE ClassTeacher.TeacherID = tea.TeacherID 
                            AND ClassTeacher.IsDeleted Is NULL  
                            AND Setting.IsDeleted Is NULL
                            AND Class.IsDeleted Is NULL
                            LIMIT 1
		                  ) As ClassID 
		                ,tea.SchoolID 
		                ,tea.isAdmin
                        ,(tea.TeacherUsername) As Username 
                        ,(SELECT  SchoolName FROM SCHOOL sse WHERE sse.SchoolID = se.SchoolID
                            AND IsDeleted Is NULL LIMIT 1) As SchoolName 
		                ,se.TermID 
		                ,ter.Year As CurrentTermYear 
		                ,ter.TermNumber As CurrentTermNumber 
                        ,tea.TeacherFullname
                        ,sc.SchoolCode 
                        , (
                                SELECT  ste.WebToken FROM Auth ste WHERE ste.TeacherID = tea.TeacherID  
                            AND IsDeleted Is NULL LIMIT 1
                            )  As WebToken 
                        FROM Teacher As tea
                        INNER JOIN School sc ON sc.SchoolID = tea.SchoolID 
                        INNER JOIN Setting se ON se.SchoolID = tea.SchoolID 
                        INNER JOIN Term ter ON ter.TermID = se.TermID
		                WHERE tea.TeacherID = @teacherID   
                        AND tea.IsDeleted Is NULL
                        AND sc.IsDeleted Is NULL
                        AND se.IsDeleted Is NULL
                        AND ter.IsDeleted Is NULL
                        ";

            using (var connection = GetConnection())
            {
                var Login = connection.Query<iGrade.Domain.Dto.LoggedUser>(sql, new { teacherID = teacherID }).FirstOrDefault();
                return Login;
            }
        }

        public iGrade.Domain.Dto.LoggedUser SessionSettingByToken(string token, ref bool dbFlag)
        {
            
            var sql = @"Select  tea.TeacherID 
		                ,
		                (
			                SELECT  ClassTeacher.ClassID
			                FROM ClassTeacher 
			                INNER JOIN Class on Class.ClassID = ClassTeacher.ClassID
			                INNER JOIN Setting on Setting.TermID = ClassTeacher.TermID
			                WHERE ClassTeacher.TeacherID = tea.TeacherID  LIMIt 1
		                  ) As ClassID 
		                ,tea.SchoolID 
		                ,tea.isAdmin
                        ,tea.TeacherUsername As Username 
                        ,(SELECT  SchoolName FROM SCHOOL sse WHERE sse.SchoolID = se.SchoolID    LIMIT 1 )As SchoolName 
		                ,se.TermID 
		                ,ter.Year As CurrentTermYear 
		                ,ter.TermNumber As CurrentTermNumber 
                        ,tea.TeacherFullname
                        ,sc.SchoolCode
                        ,(SELECT  WebToken FROM Auth ddd WHERE ddd.TeacherID = tea.TeacherID  LIMIT 1 )As WebToken 
                        ,(SELECT  LastLogin FROM Auth cds WHERE cds.TeacherID = tea.TeacherID  LIMIT 1 )As LastLogin 
                        FROM Teacher As tea
                        INNER JOIN School sc ON sc.SchoolID = tea.SchoolID 
                        INNER JOIN Setting se ON se.SchoolID = tea.SchoolID 
                        INNER JOIN Term ter ON ter.TermID = se.TermID
                        INNER JOIN Auth au ON tea.TeacherID = au.TeacherID WHERE au.WebToken = @token  LIMIT 1 ";

            using (var connection = GetConnection())
            {
                var Login = connection.Query<iGrade.Domain.Dto.LoggedUser>(sql, new { token = token }).FirstOrDefault();
                return Login;
            }
        }

        public bool PasswordChange(Guid teacherID, string newPassword, string forgotPassowrdCode, ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var date = DateTime.Today;
                    newPassword = HashPassword(newPassword);
                    var update = @"
                                UPDATE  Auth
					                            SET  Password = @newPassword,
                                                ForgotPassowrdCode = @ForgotPassowrdCode    , 
					                              isExpired = 0  , 
					                              CreatedDate = @date   
					                            WHERE TeacherID = @teacherID  
                                                AND IsDeleted IS NULL
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     teacherID = teacherID,
                                     newPassword = newPassword,
                                     ForgotPassowrdCode = forgotPassowrdCode,
                                     date = date
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

        
        public bool PasswordChange_Admin(string username, string newPassword, ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var date = DateTime.Today;
                    newPassword = HashPassword(newPassword);
                    var update = @"
                                UPDATE  Admin
					                SET  Password = @newPassword
					                WHERE Username = @username  
                                                AND IsDeleted IS NULL
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     username = username
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

        public bool PasswordChange_Parent(string email, string newPassword, ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var date = DateTime.Today;
                    newPassword = HashPassword(newPassword);
                    var update = @"
                                UPDATE  Parent
					                SET  Password = @newPassword
					                WHERE ParentEmail = @email  
                                                AND IsDeleted IS NULL
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     email = email
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





        public bool ForgotPasswordCodeUpdate(Guid teacherID, string ForgotPassowrdCode, bool isExpired, ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var date = DateTime.Today;
                    var update = @"
                                UPDATE  Auth
					                            SET  ForgotPassowrdCode = @ForgotPassowrdCode    , 
					                              isExpired = 0  , 
					                              CreatedDate = @date
					                            WHERE TeacherID = @teacherID  
                                                AND IsDeleted IS NULL
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     teacherID = teacherID,
                                     ForgotPassowrdCode = ForgotPassowrdCode,
                                     date = date
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

        public bool UpdateOneTimePin(Guid teacherID, string oneTimePin, ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {

                   var  newHasOnetimePin = HashPassword(oneTimePin);
                    var date = DateTime.Today;
                    var update = @"
                                UPDATE  Auth
					                            SET  LastLogin = @date  , 
					                              OneTimePin = @oneTimePin
					                            WHERE TeacherID = @teacherID  
                                                AND IsDeleted IS NULL 
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     teacherID = teacherID,
                                     oneTimePin = newHasOnetimePin,
                                     date = DateTime.Now
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



        public bool UpdateLastLoginDateAndToken(Guid teacherID, string newToken, ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var date = DateTime.Today;
                    var update = @"
                                UPDATE  Auth
					                            SET  LastLogin = @date  , 
					                              WebToken = @newToken
					                            WHERE TeacherID = @teacherID  
                                                AND IsDeleted IS NULL 
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     teacherID = teacherID,
                                     newToken = newToken , 
                                     date = DateTime.Now
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

        public Auth GetForgotPasswordCode(Guid teacherId, ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {

                    var update = @"Select * FROM Auth WHERE TeacherID = @teacherID 
                                                AND IsDeleted IS NULL  
                                ";
                    var id = connection.Query<Auth>(update,
                                 new
                                 {
                                     teacherID = teacherId
                                 }).FirstOrDefault();

                    return id;
                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
            }
            return null;
        }


        public Auth GetAuthByForgotPassword(string forgotPassowrdCode, ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {

                    var update = @"Select * FROM Auth WHERE ForgotPassowrdCode = @forgotPassowrdCode   
                                                AND IsDeleted IS NULL 
                                ";
                    var id = connection.Query<Auth>(update,
                                 new
                                 {
                                     forgotPassowrdCode = forgotPassowrdCode
                                 }).FirstOrDefault();

                    return id;
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
