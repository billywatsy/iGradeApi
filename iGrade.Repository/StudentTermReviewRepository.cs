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
    public class StudentTermReviewRepository : BaseRepository
    { 
        /// <summary>
        /// a teacher can not have more than tho
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="dbFlag"></param>
        /// <returns></returns>
        public List<StudentTermReviewDto> GetListByTeacherReviewsByYearAndMonth(Guid teacherId , DateTime date ,  ref bool dbFlag)
        {
            var sql = @"
SELECT StudentTermReviewID ,StudentTermRegister.studentTermRegisterID  ,Teacher.TeacherID  ,StudentTermReview.IsReviewGood    ,Body,StudentTermReview.CreatedDate ,Star5 ,
  TeacherFullname , RegNumber ,  CONCAT(Student.StudentName,'  ',Student.StudentSurname)  as  StudentFullname , Term.Year as TermYear , TermNumber
  FROM StudentTermReview
  INNER JOIN StudentTermRegister on StudentTermRegister.studentTermRegisterID = StudentTermReview.studentTermRegisterID
  INNER JOIN Student on Student.StudentID = StudentTermRegister.StudentID
  INNER JOIN Term on Term.TermID = StudentTermRegister.TermID
  INNER JOIN Teacher on Teacher.TeacherID = StudentTermReview.TeacherID
                        where StudentTermReview.TeacherId = @teacherId 
                        AND YEAR(StudentTermReview.CreatedDate) = @year
                        AND MONTH(StudentTermReview.CreatedDate) = @month
                            AND StudentTermReview.ISDELETED IS NULL 
                            AND StudentTermRegister.ISDELETED IS NULL
                            AND Student.ISDELETED IS NULL
                            AND Term.ISDELETED IS NULL
                            AND Teacher.ISDELETED IS NULL
                        Order By StudentTermReview.CreatedDate Desc
                        ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<StudentTermReviewDto>(sql,
                                                new
                                                {
                                                    teacherId = teacherId ,
                                                    year = date.Year , 
                                                    month = date.Month 
                                                }).AsList();
                return list;
            }
        }


        public List<StudentTermReviewDto> GetListByStudentTermRegisterID(Guid studentTermRegisterID, ref bool dbFlag)
        {
            var sql = @"SELECT   StudentTermReviewID ,StudentTermRegister.studentTermRegisterID  ,Teacher.TeacherID  ,StudentTermReview.IsReviewGood   ,Body,StudentTermReview.CreatedDate ,Star5 ,
  TeacherFullname , RegNumber ,Student.StudentName + ' '+ Student.StudentSurname as  StudentFullname , Term.Year as TermYear , TermNumber
  FROM StudentTermReview
  INNER JOIN StudentTermRegister on StudentTermRegister.studentTermRegisterID = StudentTermReview.studentTermRegisterID
  INNER JOIN Student on Student.StudentID = StudentTermRegister.StudentID
  INNER JOIN Term on Term.TermID = StudentTermRegister.TermID
  INNER JOIN Teacher on Teacher.TeacherID = StudentTermReview.TeacherID
                        where StudentTermReview.studentTermRegisterID IN ( SELECT studentTermRegisterID FROM StudentTermRegister Where studentTermRegisterID = @studentTermRegisterID AND ISDELETED IS NULL ) 
                        
                            AND StudentTermReview.ISDELETED IS NULL 
                            AND StudentTermRegister.ISDELETED IS NULL
                            AND Student.ISDELETED IS NULL
                            AND Term.ISDELETED IS NULL
                            AND Teacher.ISDELETED IS NULL
Order By CreatedDate Desc
                        ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<StudentTermReviewDto>(sql,
                                                new
                                                {
                                                    studentTermRegisterID = studentTermRegisterID
                                                }).AsList();
                return list;
            }
        }

        public StudentTermReviewDto GetStudentTermReview(Guid studentTermReviewId, ref bool dbFlag)
        {
            var sql = @"SELECT  StudentTermReviewID ,StudentTermRegister.studentTermRegisterID  ,Teacher.TeacherID  ,StudentTermReview.IsReviewGood   ,Body,StudentTermReview.CreatedDate ,Star5 ,
  TeacherFullname , RegNumber ,Student.StudentName + ' '+ Student.StudentSurname as  StudentFullname , Term.Year as TermYear , TermNumber
  FROM StudentTermReview
  INNER JOIN StudentTermRegister on StudentTermRegister.studentTermRegisterID = StudentTermReview.studentTermRegisterID
  INNER JOIN Student on Student.StudentID = StudentTermRegister.StudentID
  INNER JOIN Term on Term.TermID = StudentTermRegister.TermID
  INNER JOIN Teacher on Teacher.TeacherID = StudentTermReview.TeacherID
                        where StudentTermReviewID = @studentTermReviewId 
                            AND StudentTermReview.ISDELETED IS NULL 
                            AND StudentTermRegister.ISDELETED IS NULL
                            AND Student.ISDELETED IS NULL
                            AND Term.ISDELETED IS NULL
                            AND Teacher.ISDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<StudentTermReviewDto>(sql,
                                                new
                                                {
                                                    studentTermReviewId = studentTermReviewId
                                                }).FirstOrDefault();
                return list;
            }
        }

        public bool Delete(Guid studentTermReviewId,string modifiedby , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @" UPDATE StudentTermReview SET lastmodifiedby = @modifiedby ,  isdeleted = now() , islive = null  WHERE studentTermReviewId = @studentTermReviewId
                                        
                                ";
                    var id = connection.Execute(update, new
                    {
                        studentTermReviewId = studentTermReviewId ,
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

        public bool Save(StudentTermReview studentTermReview,string modifiedby , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                { 
                    var update = @"
                                INSERT INTO StudentTermReview
                                   (StudentTermReviewID
                                   ,studentTermRegisterID
                                   ,TeacherID
                                   ,IsReviewGood
                                   ,Body 
                                   ,CreatedDate 
                                   ,Star5  , lastmodifiedby
                                    )
                             VALUES
                                   (
                                    @id , 
                                    @studentTermRegisterID , 
                                    @TeacherID ,   
                                    @IsReviewGood ,  
                                    @Body ,  
                                    @dateToday,  
                                    @Star5   ,@modifiedby
                                    )
                             
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     id = Guid.NewGuid() ,
                                     StudentTermRegisterID = studentTermReview.StudentTermRegisterID,
                                     TeacherID = studentTermReview.TeacherID,
                                     IsReviewGood = studentTermReview.IsReviewGood,
                                     Body = studentTermReview.Body ,
                                     dateToday = DateTime.Today ,
                                     Star5 = studentTermReview.Star5 ,
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
    }
}
