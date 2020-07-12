using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using iGrade.Domain.Dto;

namespace iGrade.Repository
{
    public class LogRepository : BaseRepository
    {

        public static Log SaveActivity(Log log , ref bool dbError)
        {
            try
            {
                var id = Guid.NewGuid();
                var sql = @"
                                INSERT INTO Log
                                       (LogID
                                       ,TeacherID
                                       ,EntityType
                                       ,ActionType
                                       ,ActionDetails
                                       ,ActionDate
                                       )
                                 VALUES
                                       (@id
                                       ,@TeacherID 
                                       ,@EntityType 
                                       ,@ActionType 
                                       ,@ActionDetails 
                                       ,@ActionDate
                                         );
                                    ";
                var ids = GetStaticConnection().Execute(sql, new
                {
                    id = id,
                    TeacherID = log.TeacherID,
                    EntityType = log.EntityType,
                    ActionType = log.ActionType,
                    ActionDetails = log.ActionDetails,
                    ActionDate = log.ActionDate.AddMinutes(2)
                });

            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return null;
            }
            return log;
        }

        public Log Save(Log log , ref bool dbError)
        {
            try
            {
                var id = Guid.NewGuid();
                    var sql = @"
                                INSERT INTO Log
                                       (LogID
                                       ,TeacherID
                                       ,EntityType
                                       ,ActionType
                                       ,ActionDetails
                                       ,ActionDate
                                       )
                                 VALUES
                                       (@id
                                       ,@TeacherID 
                                       ,@EntityType 
                                       ,@ActionType 
                                       ,@ActionDetails 
                                       ,@ActionDate
                                         );
                                    Select @id
                                    ";
                    var ids = GetConnection().Query<Guid>(sql, new
                    {
                        id = id ,
                        TeacherID = log.TeacherID,
                        EntityType = log.EntityType ,
                        ActionType = log.ActionType ,
                        ActionDetails = log.ActionDetails ,
                        ActionDate = log.ActionDate 
                    }).FirstOrDefault(); 
                  
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return null;
            }
            return log;
        }
         
        public PagedList<LogDto> GetPagedLog(Guid schoolID, int PageSize, int PageNo, ref bool dbError)
        {
            try
            {
                if (PageNo <= 0)
                {
                    PageNo = 1;
                }
                if (PageSize <= 0)
                {
                    PageSize = 5;
                }
                var offset = (PageNo - 1) * PageSize;

                if(offset < 0)
                {
                    offset = 0;
                }
                var results = new PagedList<LogDto>();
                var sql = @"SELECT  te.TeacherUsername As Username ,
        te.TeacherFullname As TeacherName ,
        lg.EntityType ,
        lg.ActionType  ,
        lg.ActionDetails ,
        lg.ActionDate  FROM Log lg
                                          INNER JOIN Teacher te ON te.TeacherID = lg.TeacherID
						            	WHERE  te.SchoolID = @schoolID  
                                        ORDER BY lg.ActionDate 
                            LIMIT @PageSize  OFFSET @Offset ;
							
							            SELECT Count(*) FROM Log INNER JOIN Teacher ON Teacher.TeacherID = Log.TeacherID
						            	WHERE  Teacher.SchoolID = @schoolID ;
							";

                using (IDbConnection connection = GetConnection())
                {
                    using (var multi = connection.QueryMultiple(sql
                                ,
                                new
                                {
                                    schoolID = schoolID,
                                    PageSize = PageSize,
                                    OffSet = offset
                                }))
                    {
                        results.PagedData = multi.Read<LogDto>().ToList();
                        results.TotalCount = multi.Read<int>().FirstOrDefault();
                        results.Page = PageNo;
                        results.Size = PageNo;
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

        public PagedList<LogDto> GetPagedTeacherLog(Guid teacherID, int PageSize, int PageNo, ref bool dbError)
        {
            try
            {
                if (PageNo <= 0)
                {
                    PageNo = 1;
                }
                if (PageSize <= 0)
                {
                    PageSize = 5;
                }
                var offset = (PageNo - 1) * PageSize;

                if (offset < 0)
                {
                    offset = 0;
                }
                var results = new PagedList<LogDto>();
                var sql = @"SELECT  te.TeacherUsername As Username ,
        te.TeacherFullname As TeacherName ,
        lg.EntityType ,
        lg.ActionType  ,
        lg.ActionDetails ,
        lg.ActionDate  FROM Log lg
                                          INNER JOIN Teacher te ON te.TeacherID = lg.TeacherID
						            	WHERE  te.TeacherID = @teacherID   
                                        ORDER BY lg.ActionDate 
                                        LIMIT @PageSize  OFFSET @Offset ;
							
							            SELECT Count(*) FROM Log INNER JOIN Teacher ON Teacher.TeacherID = Log.TeacherID
						            	WHERE  Teacher.TeacherID = @teacherID ;
							";

                using (IDbConnection connection = GetConnection())
                {
                    using (var multi = connection.QueryMultiple(sql
                                ,
                                new
                                {
                                    teacherID = teacherID,
                                    PageSize = PageSize,
                                    OffSet = (PageNo - 1) * PageSize
                                }))
                    {
                        results.PagedData = multi.Read<LogDto>().ToList();
                        results.TotalCount = multi.Read<int>().FirstOrDefault();
                        results.Page = PageNo;
                        results.Size = PageNo;
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
