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
    public class DashboardRepository : BaseRepository
    {
        public DashboardCountDto GetDashboardCountBySchoolID(Guid schoolID, Guid termId , ref bool dbFlag)
        {
            var sql = @"SELECT 
                         (
                          SELECT Count(TeacherID) FROM Teacher Where IsActive = 1 AND sc.SchoolID = Teacher.SchoolID
                           AND IsDeleted IS NULL
                         ) As Teacher ,
                         (
                          SELECT Count(StudentTermRegister.StudentTermRegisterID) FROM StudentTermRegister 
                          INNER JOIN Student on  Student.StudentId = StudentTermRegister.StudentId
                          Where Student.IsDeleted IS NULL AND Student.SchoolID = @schoolID
                          AND StudentTermRegister.TermID = @termId
                         ) As StudentTermRegister , 
                         (
                         SELECT count(CommitteMemberID) FROM CommitteMember Where SchoolID = sc.SchoolID
                           AND IsDeleted IS NULL
                         ) As CommitteMember ,
                         (
                         SELECT count(SubjectID) FROM Subject Where SchoolID = sc.SchoolID
                           AND IsDeleted IS NULL
                         ) As Subject
                         ,
                         (
                         SELECT count(ClassID) FROM Class Where SchoolID = sc.SchoolID
                           AND IsDeleted IS NULL
                         ) As Class
                         ,
                         (
                             SELECT  
                            count(studenttermregister.StudentTermRegisterID) as Male 
                            FROM `studenttermregister`  
                            INNER JOIN term on term.TermID = studenttermregister.TermID
                            INNER JOIN student on student.StudentID = studenttermregister.StudentID
                            WHERE student.SchoolID = @schoolID 
                            AND studenttermregister.TermID =  @termId 
                            AND student.IsMale = 1 
                            AND studenttermregister.IsDeleted IS NULL
                            AND term.IsDeleted IS NULL
                            AND student.IsDeleted IS NULL
                         )
                            AS Male ,
                         (
                             SELECT  
                            count(studenttermregister.StudentTermRegisterID) as Male 
                            FROM `studenttermregister`  
                            INNER JOIN term on term.TermID = studenttermregister.TermID
                            INNER JOIN student on student.StudentID = studenttermregister.StudentID
                            WHERE student.SchoolID = @schoolID
                            AND studenttermregister.TermID = @termId  
                            AND student.IsMale = 0 
                            AND studenttermregister.IsDeleted IS NULL
                            AND term.IsDeleted IS NULL
                            AND student.IsDeleted IS NULL
                        ) As Female
                        FROM School sc
                        Where sc.SchoolID = @schoolID
                        AND sc.IsDeleted IS NULL
                        ;";

            using (var connection = GetConnection())
            {
                try
                {
                    var list = connection.Query<DashboardCountDto>(sql, new { schoolID = schoolID , termId= termId })
                                         .FirstOrDefault();
                    return list;
                }
                catch (Exception er)
                {
                    return null;
                }
            }
        }

        public List<NumberRegisteredPerTermDto> GetNumberRegisteredBySchoolPerTerm(Guid schoolID, ref bool dbFlag)
        {
            var sql = @" SELECT count(studenttermregister.StudentTermRegisterID) as NumberRegistered , 
                        term.Year , 
                        term.TermNumber ,
                        term.startDate
                        FROM `studenttermregister`  
                        INNER JOIN term on term.TermID = studenttermregister.TermID
                        INNER JOIN student on student.StudentID = studenttermregister.StudentID
                        WHERE student.SchoolID = @schoolID 
                        AND studenttermregister.IsDELETED IS NULL
                        AND term.IsDELETED IS NULL
                        AND student.IsDELETED IS NULL
                        GROUP BY studenttermregister.TermID";

            using (var connection = GetConnection())
            {
                var list = connection.Query<NumberRegisteredPerTermDto>(sql, new { schoolID = schoolID })
                                     .AsList();
                return list;
            }
        }
        
    }
}
