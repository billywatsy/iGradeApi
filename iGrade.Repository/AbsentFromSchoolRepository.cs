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
    public class AbsentFromSchoolRepository : BaseRepository
    {
        public bool Delete(Guid id , string modifiedBy, ref bool  dbFlag)
        {
            try
            {
                using (var connection = GetConnection())
                {

                    var update = @" UPDATE AbsentFromSchool SET  isdeleted = now() , islive = null , LastModifiedBy  = @modifiedBy WHERE AbsentFromSchoolID = @id   
                                ";
                    var count = connection.Execute(update,
                                 new
                                 {
                                     id = id ,
                                     modifiedBy = modifiedBy
                                 });
                    if (count >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception er)
            {
                dbFlag = true;
                DbLog.Error(er);
                return false;
            }
        }
        public bool Save(AbsentFromSchool absent, string modifiedby , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    
                        var update = @"
                                INSERT INTO AbsentFromSchool
                                   (AbsentFromSchoolID ,
                                    StudentTermRegisterID
                                   ,DayAbsent
                                   ,Reason  ,
                                    DateCreated , LastModifiedBy
                                    )
                             VALUES
                                   (  @AbsentFromSchoolID ,
                                    @StudentTermRegisterID , 
                                    @DayAbsent  
                                   ,@Reason  
                                    ,@DateCreated , @modifiedby
                                  ) 
                                ";
                        var id = connection.Execute(update,
                                     new
                                     {
                                         AbsentFromSchoolID = Guid.NewGuid() , 
                                         StudentTermRegisterID = absent.StudentTermRegisterID,
                                         DayAbsent = absent.DayAbsent,
                                         Reason = absent.Reason ,
                                         DateCreated = DateTime.Now ,
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
        public List<AbsentFromSchoolDto> GetListAbsentByStudentTermRegisterID(Guid StudentTermRegisterID, ref bool dbFlag)
        {
            var sql = @"SELECT ab.absentFromSchoolID , 
                               stu.RegNumber ,
                               CONCAT(stu.StudentName,'  ',stu.StudentSurname) as StudentName,
                               ab.dayAbsent,
                               ab.reason 
                        FROM AbsentFromSchool ab
                        INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ab.StudentTermRegisterID 
                        INNER JOIN Student stu on stu.StudentID = enrol.StudentID 
                        AND enrol.StudentTermRegisterID = @StudentTermRegisterID 
                        WHERE ab.IsDeleted IS NULL
                        AND enrol.IsDeleted IS NULL
                        AND stu.IsDeleted IS NULL ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<AbsentFromSchoolDto>(sql, new { StudentTermRegisterID = StudentTermRegisterID }).AsList();
                return list;
            }
        }

        public AbsentFromSchoolDto GetByID(Guid id, ref bool dbFlag)
        {
            var sql = @"SELECT ab.absentFromSchoolID , 
                               stu.RegNumber ,
                               CONCAT(stu.StudentName,'  ',stu.StudentSurname) as StudentName,
                               ab.dayAbsent,
                               ab.reason 
                        FROM AbsentFromSchool ab
                        INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ab.StudentTermRegisterID 
                        INNER JOIN Student stu on stu.StudentID = enrol.StudentID 
                        Where ab.AbsentFromSchoolID =  @absentFromSchoolID 
                        AND ab.IsDeleted IS NULL
                        AND enrol.IsDeleted IS NULL
                        AND stu.IsDeleted IS NULL ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<AbsentFromSchoolDto>(sql, new { absentFromSchoolID = id }).FirstOrDefault();
                return list;
            }
        }

        public List<AbsentFromSchoolDto> GetListAbsentByClassIDAndTermID(Guid classID,Guid termID , ref bool dbFlag)
        {
            var sql = @"SELECT ab.absentFromSchoolID , 
                               stu.RegNumber ,
                               CONCAT(stu.StudentName,'  ',stu.StudentSurname) as StudentName,
                               ab.dayAbsent,
                               ab.reason 
                        FROM AbsentFromSchool ab
                        INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ab.StudentTermRegisterID 
                        INNER JOIN Student stu on stu.StudentID = enrol.StudentID 
                        WHERE enrol.ClassID = @classID 
                        AND enrol.TermID = @termID 
                        AND ab.IsDeleted IS NULL
                        AND enrol.IsDeleted IS NULL
                        AND stu.IsDeleted IS NULL

                        ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<AbsentFromSchoolDto>(sql, new { classID = classID  , termID  = termID }).AsList();
                return list;
            }
        }
        public List<AbsentFromSchoolDto> GetListAbsentBySchoolAndYearAndMonth(Guid schoolID, DateTime date, ref bool dbFlag)
        {
            var sql = @"SELECT ab.absentFromSchoolID , 
                               stu.RegNumber ,
                               CONCAT(stu.StudentName,'  ',stu.StudentSurname) as StudentName,
                               ab.dayAbsent,
                               ab.reason 
                        FROM AbsentFromSchool ab
                        INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ab.StudentTermRegisterID 
                        INNER JOIN Student stu on stu.StudentID = enrol.StudentID 
                        WHERE stu.SchoolID = @schoolID 
                        AND YEAR(ab.DayAbsent) = YEAR(@date)
                        AND MONTH(ab.DayAbsent) = MONTH(@date)
                        AND ab.IsDeleted IS NULL
                        AND enrol.IsDeleted IS NULL
                        AND stu.IsDeleted IS NULL 
                        ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<AbsentFromSchoolDto>(sql, new { schoolID = schoolID, date = date }).AsList();
                return list;
            }
        }



        public List<AbsentFromSchoolDto> GetListAbsentBySchoolAndDateRange(Guid schoolID, DateTime dateStart, DateTime dateEnd , ref bool dbFlag)
        {
            var sql = @"SELECT  ab.absentFromSchoolID , 
                               stu.RegNumber ,
                               CONCAT(stu.StudentName,'  ',stu.StudentSurname) as StudentName,
                               ab.dayAbsent,
                               ab.reason 
                        FROM AbsentFromSchool ab
                        INNER JOIN StudentTermRegister enrol on enrol.StudentTermRegisterID = ab.StudentTermRegisterID 
                        INNER JOIN Student stu on stu.StudentID = enrol.StudentID 
                        WHERE stu.SchoolID = @schoolID 
                        AND ab.DayAbsent BETWEEN @dateStart  AND @dateEnd 
                        AND ab.IsDeleted IS NULL
                        AND enrol.IsDeleted IS NULL
                        AND stu.IsDeleted IS NULL
                        ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<AbsentFromSchoolDto>(sql, new { schoolID = schoolID, dateStart = dateStart , dateEnd  = dateEnd }).AsList();
                return list;
            }
        }

    }
}
