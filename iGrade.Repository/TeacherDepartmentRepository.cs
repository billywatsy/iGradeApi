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
    public class TeacherDepartmentRepository : BaseRepository
    {
        public List<TeacherDepartmentDto> GetListBySchoolID(Guid schoolID, ref bool dbFlag)
        {
            var sql = @"SELECT 
                        TeacherDepartmentId ,
                        Department.DepartmentId ,
                        Teacher.TeacherId ,
                        Teacher.TeacherUsername ,
                        Teacher.TeacherFullname ,
                        IsHeadOfDepartment ,
                        Department.Code As  DepartmentCode ,
                        Department.Name As  DepartmentName 		
                        FROM TeacherDepartment 
                        inner join Department  on Department.DepartmentId = TeacherDepartment.DepartmentId
                        inner join Teacher  on Teacher.TeacherId = TeacherDepartment.TeacherId 
                        WHERE Teacher.SchoolID = @schoolID
                        AND Department.SchoolID = @schoolID 
                        And TeacherDepartment.IsDeleted IS NULL
                        ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<TeacherDepartmentDto>(sql, new { schoolID = schoolID }).AsList();
                return list;
            }
        }

        public List<TeacherDepartmentDto> GetListByDepartmentID(Guid departmentId, ref bool dbFlag)
        {
            var sql = @"SELECT 
                        TeacherDepartmentId ,
                        Department.DepartmentId ,
                        Teacher.TeacherId ,
                        Teacher.TeacherUsername ,
                        Teacher.TeacherFullname ,
                        IsHeadOfDepartment ,
                        Department.Code As  DepartmentCode ,
                        Department.Name As  DepartmentName 		
                        FROM TeacherDepartment 
                        inner join Department  on Department.DepartmentId = TeacherDepartment.DepartmentId
                        inner join Teacher  on Teacher.TeacherId = TeacherDepartment.TeacherId 
                        WHERE   Department.departmentId = @departmentId 
                       And TeacherDepartment.IsDeleted IS NULL
                        ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<TeacherDepartmentDto>(sql, new { departmentId = departmentId }).AsList();
                return list;
            }
        }

        public List<TeacherDepartmentDto> GetListByTeacherId(Guid teacherID, ref bool dbFlag)
        {
            var sql = @"SELECT 
                        TeacherDepartmentId ,
                        Department.DepartmentId ,
                        Teacher.TeacherId ,
                        Teacher.TeacherUsername ,
                        Teacher.TeacherFullname ,
                        IsHeadOfDepartment ,
                        Department.Code As  DepartmentCode ,
                        Department.Name As  DepartmentName 		
                        FROM TeacherDepartment 
                        inner join Department  on Department.DepartmentId = TeacherDepartment.DepartmentId
                        inner join Teacher  on Teacher.TeacherId = TeacherDepartment.TeacherId 
                        WHERE   TeacherDepartment.TeacherId = @teacherID 
                        And TeacherDepartment.IsDeleted IS NULL
                        ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<TeacherDepartmentDto>(sql, new { teacherID = teacherID }).AsList();
                return list;
            }
        }

        public bool Save(TeacherDepartment objClass, string modifiedBy ,ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                { 
                    var update = @"UPDATE TeacherDepartment SET TeacherID = @TeacherID ,
                                    DepartmentId = @DepartmentId  , 

                                    LastModifiedBy = @modifiedBy
                                                                    WHERE teacherDepartmentId = @teacherDepartmentId
            
                                    AND IsDeleted Is NULL
                                ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     DepartmentId = objClass.DepartmentId,
                                     TeacherID = objClass.TeacherId,
                                     teacherDepartmentId = objClass.TeacherDepartmentId,
                                     modifiedBy = modifiedBy,
                                 });

                    if (id <= 0)
                    {
                        var insert = @" 
                                            INSERT INTO TeacherDepartment
                                               (
                                                DepartmentId ,
                                                teacherDepartmentId ,
                                               TeacherID , 
                                                LastModifiedBy
                                                )
                                             VALUES
                                               (
                                                @DepartmentId,  
                                                @teacherDepartmentId , 
                                                @TeacherID  ,
                                                @modifiedBy
                                                ) 
                                ";
                        var idI = connection.Execute(insert,
                                     new
                                     {
                                         DepartmentId = objClass.DepartmentId,
                                         TeacherID = objClass.TeacherId,
                                         teacherDepartmentId = Guid.NewGuid(),
                                         modifiedBy = modifiedBy,
                                     });

                    }
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

        public bool Delete(Guid teacherDepartmentId, string modifiedBy  , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                { 
                    var update = @"  
                                     UPDATE  TeacherDepartment   SET  isdeleted = now() , islive = null ,
                                     LastModifiedBy = @modifiedBy
                                     where 
                                     teacherDepartmentId = @teacherDepartmentId  
                                "; 
                    var id = connection.Execute(update, new
                    {
                        teacherDepartmentId = teacherDepartmentId ,
                        modifiedBy = modifiedBy
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
