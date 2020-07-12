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
    public class LessonPlanRepository : BaseRepository
    {
        public bool Delete(Guid lessonPlanId, string modifiedby, ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @" UPDATE  LessonPlan SET lastmodifiedby = @modifiedby , isdeleted = now() , islive = null  WHERE LessonPlanId = @lessonPlanId
                                ";
                    var id = connection.Query<int>(update, new
                    {
                        lessonPlanId = lessonPlanId,
                        modifiedby = modifiedby
                    }).FirstOrDefault();

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

        public LessonPlanDto GetByLessonPlanID(Guid lessonPlanID, ref bool dbFlag)
        {
            var sql = @"SELECT ex.LessonPlanId, ex.TeacherClassSubjectId ,  ex.DateStart, ex.Body ,
                            ex.DateEnd, ex.AfterLessonComment, ex.ApprovedByID, ex.CreatedDate, 
                            ex.IsDeleted, ex.LastModifiedBy , Teacher.TeacherFullname as ApprovedByName
                        FROM LessonPlan ex 
                        LEFT JOIN Teacher on Teacher.TeacherId = ex.ApprovedByID
                        where ex.LessonPlanID =  @lessonPlanID
                        AND ex.IsDeleted IS NULL
                          ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<LessonPlanDto>(sql,
                                                new
                                                {
                                                    lessonPlanID = lessonPlanID
                                                })
                                 .FirstOrDefault();
                return list;
            }
        }

        public List<LessonPlanDto> GetListLessonPlansByTeacherClassSubjectID(Guid teacherClassSubjectID, ref bool dbFlag)
        {
            var sql = @"SELECT LessonPlanId, TeacherClassSubjectId ,  DateStart, DateEnd, AfterLessonComment, ApprovedByID, CreatedDate, IsDeleted, LastModifiedBy
                           FROM LessonPlan 
                          where TeacherClassSubjectId = @TeacherClassSubjectId
                        AND IsDeleted IS NULL
                          ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<LessonPlanDto>(sql,
                                                new
                                                {
                                                    TeacherClassSubjectId = teacherClassSubjectID
                                                }
                                                )
                                 .AsList();
                return list;
            }
        }


        public LessonPlan Save(LessonPlan lessonPlan, string modifiedBy, ref bool dbFlag)
        {
            using (var connection = GetConnection())
            {
                if (lessonPlan.LessonPlanId == null || lessonPlan.LessonPlanId == Guid.Empty)
                {
                    lessonPlan.LessonPlanId = Guid.NewGuid();
                    var update = @"
                                INSERT INTO LessonPlan
                                   (LessonPlanID
                                   ,TeacherClassSubjectId
                                   ,Body
                                   ,DateStart 
                                   ,DateEnd 
                                   ,AfterLessonComment
                                   ,ApprovedByID
                                   ,lastmodifiedBy
                                    )
                             VALUES
                                   (
                                    @id , 
                                    @TeacherClassSubjectId , 
                                    @Body , 
                                    @DateStart ,
                                    @DateEnd ,
                                    @AfterLessonComment ,
                                    @ApprovedByID ,
                                    @modifiedBy
                                   ) 
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     id = lessonPlan.LessonPlanId,
                                     TeacherClassSubjectId = lessonPlan.TeacherClassSubjectId,
                                     Body = lessonPlan.Body,
                                     DateStart = lessonPlan.DateStart,
                                     DateEnd = lessonPlan.DateEnd,
                                     AfterLessonComment = lessonPlan.AfterLessonComment,
                                     ApprovedByID = lessonPlan.ApprovedByID,
                                     modifiedBy = modifiedBy
                                 });

                    return lessonPlan;
                }
                else
                {

                    var update = @"         UPDATE LessonPlan
					                        SET  Body = @Body   
					                        ,  DateStart = @DateStart  
					                        ,  DateEnd = @DateEnd  
					                        ,  AfterLessonComment = @AfterLessonComment  
                                            ,  lastmodifiedBy = @modifiedBy 
					                        WHERE   LessonPlanId = @id 
AND ISDeleted IS NULL
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     id = lessonPlan.LessonPlanId,
                                     Body = lessonPlan.Body,
                                     DateStart = lessonPlan.DateStart,
                                     DateEnd = lessonPlan.DateEnd,
                                     AfterLessonComment = lessonPlan.AfterLessonComment,
                                     modifiedBy = modifiedBy
                                 });

                    return lessonPlan;
                }

            }



        }

        public bool SaveApproval(Guid lessonPlanId,Guid approvedById,  string modifiedBy, ref bool dbFlag)
        {
            using (var connection = GetConnection())
            {
                var update = @"         UPDATE LessonPlan
					                        SET  ApprovedByID = @ApprovedByID  
					                        WHERE   LessonPlanId = @id 
                                        AND ISDeleted IS NULL
                                ";
                var id = connection.Execute(update,
                             new
                             {
                                 id = lessonPlanId,
                                 ApprovedByID = approvedById, 
                                 modifiedBy = modifiedBy
                             });
                 
                return true;
            }
        }
    }
}