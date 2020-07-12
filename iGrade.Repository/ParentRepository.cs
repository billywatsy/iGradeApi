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
    public class ParentRepository : BaseRepository
    {
        public Parent GetByEmail(string email, ref bool dbFlag)
        {
            var sql = @"SELECT 
                        FROM Parent 
                        Where ParentEmail = @email
                        AND ISDeleted Is null
                        ;
                        ";

            using (var connection = GetConnection())
            {
                try
                {
                    var list = connection.Query<Parent>(sql, new { email = email })
                                         .FirstOrDefault();
                    return list;
                }
                catch (Exception er)
                {
                    dbFlag = true;
                    return null;
                }
            }
        }

        public Parent GetByWebToken(string WebToken, ref bool dbFlag)
        {
            var sql = @"SELECT *
                        FROM Parent 
                        Where WebToken = @WebToken
                        AND ISDeleted Is null
                        ;
                        ";

            using (var connection = GetConnection())
            {
                try
                {
                    var list = connection.Query<Parent>(sql, new { WebToken = WebToken })
                                         .FirstOrDefault();
                    return list;
                }
                catch (Exception er)
                {
                    dbFlag = true;
                    return null;
                }
            }
        }


        public bool IsSchoolCodeAndEmailExist(string email, string schoolCode , ref bool dbFlag)
        {
            var sql = @"SELECT * 
                        FROM Parent 
                        Where ParentEmail = @email 
                        AND schoolCode = @code 
                        ;
                        ";

            using (var connection = GetConnection())
            {
                try
                {
                    var list = connection.Query<Parent>(sql, new { email = email  , code = schoolCode })
                                         .FirstOrDefault();
                    if (list != null)
                    {
                        return true;
                    }
                }
                catch (Exception er)
                {
                    dbFlag = true;
                }
            }
            return false;
        }






        public Parent Login(string email, string schoolCode, string password, ref bool dbFlag)
        {
            var sql = @"SELECT *
                        FROM Parent 
                        Where ParentEmail = @email
                        AND Password = @password
                        AND SchoolCode  = @schoolCode
AND ISDeleted Is null
                        ;
                        ";

            using (var connection = GetConnection())
            {
                try
                {
                    var list = connection.Query<Parent>(sql, new { email = email, password = password, schoolCode = schoolCode })
                                         .FirstOrDefault();

                    if (list != null)
                    {
                        if ((DateTime.Today - list.WebTokenLastUpdated).TotalDays > 14)
                        {
                            var token = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

                            var update = @"
                                UPDATE Parent
					                            SET  WebToken = @token  , WebTokenLastUpdated = now()
					                            WHERE ParentEmail = @email
					                            AND SchoolCode = @schoolCode
                                AND IsDeleted IS NULL
                                ";
                            var id = connection.Execute(update,
                                         new
                                         {
                                             email = email,
                                             schoolCode = schoolCode,
                                             token = token
                                         });
                            list.WebToken = token;
                        }
                    }
                    else
                    {
                        var sqlOneTimePin = @"SELECT *
                        FROM Parent 
                        Where ParentEmail = @email
                        AND OneTimePin = @password
                        AND SchoolCode  = @schoolCode
                        AND ISDeleted Is null
                        ;
                        ";

                        list = connection.Query<Parent>(sqlOneTimePin, new { email = email, password = password, schoolCode = schoolCode })
                                         .FirstOrDefault();

                        if (list != null)
                        {
                            var resetPin = Guid.NewGuid().ToString().Substring(0, 3);
                            var update = @"
                                UPDATE Parent
					                            SET  OneTimePin = @resetPin  
					                            WHERE ParentEmail = @email
					                            AND SchoolCode = @schoolCode
                                AND IsDeleted IS NULL
                                ";
                            var id = connection.Execute(update,
                                         new
                                         {
                                             email = email,
                                             schoolCode = schoolCode,
                                             resetPin = resetPin
                                         });
                        }
                    }
                    return list;
                }
                catch (Exception er)
                {
                    dbFlag = true;
                    return null;
                }
            }
        }

        public bool UpdateOneTimePin(string email, string schoolCode, string resetPin, ref bool dbFlag)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    var update = @"
                                UPDATE Parent
					                            SET  OneTimePin = @resetPin  
					                            WHERE ParentEmail = @email
					                            AND SchoolCode = @schoolCode
                                AND IsDeleted IS NULL
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     email = email,
                                     schoolCode = schoolCode,
                                     resetPin = resetPin
                                 });
                    return true;
                }
                catch (Exception er)
                {
                    dbFlag = true;
                    return false;
                }
            }
        }


        public bool Register(string email, string schoolCode, string password, ref bool dbFlag)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    var token = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
                    var update = @"INSERT INTO `parent`(`ParentEmail`, `SchoolCode`, `Password`,
                                `OneTimePin`, `MobileToken`, `WebToken`, `WebTokenLastUpdated`, 
                                `IsEmailVerified`, `IsDeleted`, `LastModifiedBy`)
                                VALUES 
                                (@email,@schoolCode,@password,@password,@password,@password,now(),1,NULL,NULL)
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     email = email,
                                     schoolCode = schoolCode,
                                     password = password
                                 });
                    return true;
                }
                catch (Exception er)
                {
                    dbFlag = true;
                    return false;
                }
            }
        }

    }
}
