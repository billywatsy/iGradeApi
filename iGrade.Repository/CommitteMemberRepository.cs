using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace iGrade.Repository
{
    public class CommitteMemberRepository : BaseRepository
    {

        public List<CommitteMember> GetListCommitteMemberBySchoolID(Guid schoolID, ref bool dbFlag)
        {
            var sql = @"SELECT *
                         FROM CommitteMember  
                         WHERE  SchoolID = @schoolID AND ISDeleted IS NULL";

            using (var connection = GetConnection())
            {
                try
                {
                    var list = connection.Query<CommitteMember>(sql , new { schoolID = schoolID }    
                               ).AsList();
                    return list;
                }
                catch (Exception er)
                {
                    return null;
                }
            }
        }

        public CommitteMember GetCommitteMemberByID(Guid committeMemberID, ref bool dbFlag)
        {
            var sql = @"SELECT *
                         FROM CommitteMember
                         WHERE CommitteMemberID = @committeMemberID  AND ISDeleted IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<CommitteMember>(sql , new { committeMemberID = committeMemberID } ).FirstOrDefault();
                return list;
            }
        }

        public CommitteMember Save(CommitteMember objCommiteMember,string modifiedBy , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    if (objCommiteMember.CommitteMemberID == null )
                    {
                        objCommiteMember.CommitteMemberID = Guid.NewGuid();
                        var update = @"
                                 INSERT INTO CommitteMember
                               (
                                CommitteMemberID
                               ,SchoolID
                               ,Title
                               ,Fullname
                               ,Phone
                               ,Email
                               ,LastModifiedBy
                               )
                         VALUES
                               (
                                @id
                               ,@schoolID
                               ,@title
                               ,@Fullname
                               ,@phone
                               ,@email 
                               ,@modifiedBy
                                )
                                ";
                        var id = connection.Execute(update,
                                     new
                                     { 
                                         id = objCommiteMember.CommitteMemberID,
                                        schoolID = objCommiteMember.SchoolID ,
                                        title = objCommiteMember.Title ,
                                        fullname = objCommiteMember.Fullname ,
                                        phone = objCommiteMember.Phone ,
                                        email = objCommiteMember.Email ,
                                         modifiedBy = modifiedBy
                                     });
                        
                    }
                    else
                    {
                        var update = @"
                                UPDATE   CommitteMember
					            SET  Title = @title   
					                ,  Fullname = @fullname   
					                ,  Phone = @phone   
					                ,  Email = @email   
,LastModifiedBy = @modifiedBy
					            WHERE CommitteMemberID = @committeMemberID
AND ISDeleted IS NULL
                                ";
                        var id = connection.Execute(update,
                                     new
                                     {
                                         committeMemberID = objCommiteMember.CommitteMemberID,
                                         schoolID = objCommiteMember.SchoolID,
                                         title = objCommiteMember.Title,
                                         fullname = objCommiteMember.Fullname,
                                         phone = objCommiteMember.Phone,
                                         email = objCommiteMember.Email ,
                                         modifiedBy = modifiedBy
                                     });
                        
                    }
                    return objCommiteMember;
                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return null;
            }
        }

        public bool Delete(Guid committeMemberID, string modifiedBy , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @"  UPDATE CommitteMember SET  isdeleted = now() , islive = null ,
                                     LastModifiedBy = @modifiedBy WHERE CommitteMemberID = @committeMemberID  
AND ISDELETED IS NULL;
                                ";
                    var id = connection.Execute(update, new
                    {
                        committeMemberID = committeMemberID ,
                        modifiedBy = modifiedBy
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
    }
}
