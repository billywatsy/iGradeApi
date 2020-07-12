using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace iGrade.Repository
{
    public class DepartmentRepository : BaseRepository
    {

        public List<Department> GetListDepartments(Guid schoolId, ref bool dbFlag)
        {
            var sql = @"SELECT  *
                         FROM Department
                        where SchoolID = @schoolID AND ISDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<Department>(sql,
                                                new
                                                {
                                                    schoolID = schoolId
                                                }).AsList();
                return list;
            }
        }

        public Department GetDepartment(Guid departmentId, ref bool dbFlag)
        {
            var sql = @"SELECT   *
                         FROM Department
                        where DepartmentID = @departmentId AND ISDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<Department>(sql,
                                                new
                                                {
                                                    departmentID = departmentId
                                                }).FirstOrDefault();
                return list;
            }
        }

        public Department GetDepartmentByCode(string departmentCode, Guid schoolId , ref bool dbFlag)
        {
            var sql = @"SELECT   *
                         FROM Department
                        where SchoolID = @schoolId
                        AND Code = @departmentCode
AND ISDELETED IS NULL
                        ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<Department>(sql,
                                                new
                                                {
                                                    departmentCode = departmentCode ,
                                                    schoolId = schoolId
                                                }).FirstOrDefault();
                return list;
            }
        }



        public bool Delete(Guid departmentId, string modifiedby , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @"UPDATE  Department SET lastmodifiedby = @modifiedby ,  isdeleted = now() , islive = null  WHERE DepartmentId = @departmentId 
                                
    AND DepartmentID Not IN ( Select DepartmentID FROM subjectdepartment where DepartmentId = @departmentId AND  IsDeleted IS NULL)";
                    var id = connection.Execute(update, new
                    {
                        DepartmentID = departmentId ,
                        modifiedby = modifiedby
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

        public Department Save(Department department,string modifiedby , ref bool dbError)
        {

            var departmentIsExist = GetDepartmentByCode(department.Code, department.SchoolID, ref dbError);
            if (dbError)
            {
                return null;
            }
            try
            {
                using (var connection = GetConnection())
                {
                    department.Code = CleanIDcodeAlphanumeric(department.Code);

               if (departmentIsExist == null)
                {
                        department.DepartmentId = Guid.NewGuid();
                        var insert = @"
                                INSERT INTO Department
                                   (DepartmentID
                                   ,Name
                                   ,Code 
                                   ,SchoolID  ,
  LastModifiedBy
                                    )
                             VALUES
                                   (
                                    @id , 
                                    @name , 
                                    @code ,     
                                    @schoolID  ,@modifiedby 
                                    ) 
                                ";
                        var id = connection.Execute(insert,
                                     new
                                     {
                                         id = department.DepartmentId,
                                         code = department.Code,
                                         name = department.Name, 
                                         schoolID = department.SchoolID ,
                                         modifiedby = modifiedby
                                     });
                    }
                else
                {

                        var update = @" 
                                UPDATE Department
					                            SET Name = @name , lastmodifiedby = @modifiedby    
					                            WHERE DepartmentID = @id AND ISDELETED IS NULL
                                ";
                        var id = connection.Execute(update,
                                     new
                                     {
                                         id = department.DepartmentId, 
                                         name = department.Name , 
                                         modifiedby = modifiedby
                                     });
                    }


                    return department;

                }
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return null;
            }
        }
    }
}
