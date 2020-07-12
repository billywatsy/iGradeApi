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
    public class LessonPlanCommentRepository : BaseRepository
    {
        public bool Delete(Guid lessonPlanId, string modifiedby , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @" UPDATE  LessonPlanComment SET lastmodifiedby = @modifiedby , isdeleted = now() , islive = null  WHERE LessonPlanCommentId = @lessonPlanId
                                ";
                    var id = connection.Query<int>(update, new
                    {
                        lessonPlanId = lessonPlanId ,
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

        public LessonPlanComment GetByLessonPlanCommentID(Guid LessonPlanCommentID, ref bool dbFlag)
        {
            var sql = @"SELECT *
                           FROM LessonPlanComment  
                              where LessonPlanID = @LessonPlanCommentID
            
                          ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<LessonPlanComment>(sql,
                                                new
                                                {
                                                    LessonPlanCommentID = LessonPlanCommentID
                                                }
                                                )
                                 .FirstOrDefault();
                return list;
            }
        }

        public List<LessonPlanCommentDto> GetListByLessonPlanID(Guid LessonPlanID, Guid teacherId , ref bool dbFlag)
        {
            var sql = @"SELECT LessonPlanCommentId  , 
                                CASE
                              		WHEN LessonPlanComment.LastModifiedDate > LessonPlanComment.CreatedDate 
                                    THEN 1 
                                    ELSE 0  
                                END AS  IsEdited
                                 , 
                                  CASE
                              		WHEN LessonPlanComment.IsDeleted IS NULL 
                                    THEN LessonPlanComment.Comment  
                                    ELSE NULL  
                                  END AS  Comment
                                 , 
                                  CASE
                              		WHEN Teacher.TeacherID = @teacherId 
                                    THEN 1
                                    ELSE 0  
                                  END AS  IsMyComment
                                 , 
                                LessonPlanComment.IsDeleted , 
                                Teacher.TeacherFullname  , 
                                LessonPlanComment.LastModifiedDate
                           FROM LessonPlanComment 
                           INNER JOIN Teacher on Teacher.TeacherID = LessonPlanComment.TeacherID
                              where LessonPlanID = @LessonPlanID
                              ORDER BY lessonplancomment.CreatedDate DESC
                          ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<LessonPlanCommentDto>(sql,
                                                new
                                                {
                                                    LessonPlanID = LessonPlanID ,
                                                    teacherId = teacherId
                                                }
                                                )
                                 .AsList();
                return list;
            }
        }
        

        public LessonPlanComment Save(LessonPlanComment lessonPlanComment, string modifiedBy ,ref bool dbFlag)
        {
            using (var connection = GetConnection())
            {
                if(lessonPlanComment.LessonPlanCommentId == null || lessonPlanComment.LessonPlanCommentId == Guid.Empty)
                {
                    lessonPlanComment.LessonPlanCommentId = Guid.NewGuid(); 
                    var update = @"
                                INSERT INTO LessonPlanComment
                                   (LessonPlanCommentID
                                   ,LessonPlanId
                                   ,TeacherId
                                   ,Comment  
                                   ,lastmodifiedBy
                                    )
                             VALUES
                                   (
                                    @LessonPlanCommentID , 
                                    @LessonPlanId , 
                                    @TeacherId , 
                                    @Comment , 
                                    @modifiedBy
                                   ) 
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     LessonPlanCommentID = lessonPlanComment.LessonPlanCommentId,
                                     LessonPlanId = lessonPlanComment.LessonPlanId,
                                     TeacherId = lessonPlanComment.TeacherId,
                                     Comment = lessonPlanComment.Comment, 
                                     modifiedBy = modifiedBy
                                 });

                    return lessonPlanComment;
                }
                else
                {

                    var update = @"         UPDATE LessonPlanComment
					                        SET  Comment = @Comment  , 
                                            LastModifiedDate = NOW() ,
                                            LastModifiedBy	 = @modifiedBy
					                        WHERE   LessonPlanCommentId = @id 
AND ISDeleted IS NULL
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     id = lessonPlanComment.LessonPlanCommentId,
                                     Comment = lessonPlanComment.Comment ,
                                     modifiedBy = modifiedBy
                                 });

                    return lessonPlanComment;
                }

            }

        }

    }
}
