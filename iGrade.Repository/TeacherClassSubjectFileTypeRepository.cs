using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace iGrade.Repository
{
    public class TeacherClassSubjectFileTypeRepository : BaseRepository
    {

        public List<TeacherClassSubjectFileType> GetListBySchoolId(Guid schoolId, ref bool dbFlag)
        {
            var sql = @"SELECT  *
                         FROM TeacherClassSubjectFileType
                        where SchoolID = @schoolID AND ISDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<TeacherClassSubjectFileType>(sql,
                                                new
                                                {
                                                    schoolID = schoolId
                                                }).AsList();
                return list;
            }
        }

        public TeacherClassSubjectFileType GetTeacherClassSubjectFileType(Guid teacherClassSubjectFileTypeId, ref bool dbFlag)
        {
            var sql = @"SELECT   *
                         FROM TeacherClassSubjectFileType
                        where TeacherClassSubjectFileTypeID = @teacherClassSubjectFileTypeId AND ISDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<TeacherClassSubjectFileType>(sql,
                                                new
                                                {
                                                    teacherClassSubjectFileTypeID = teacherClassSubjectFileTypeId
                                                }).FirstOrDefault();
                return list;
            }
        }

        public TeacherClassSubjectFileType GetTeacherClassSubjectFileTypeByCode(string teacherClassSubjectFileTypeCode, Guid schoolId , ref bool dbFlag)
        {
            var sql = @"SELECT   *
                         FROM TeacherClassSubjectFileType
                        where SchoolID = @schoolId
                        AND Code = @teacherClassSubjectFileTypeCode
AND ISDELETED IS NULL
                        ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<TeacherClassSubjectFileType>(sql,
                                                new
                                                {
                                                    teacherClassSubjectFileTypeCode = teacherClassSubjectFileTypeCode ,
                                                    schoolId = schoolId
                                                }).FirstOrDefault();
                return list;
            }
        }



        public bool Delete(Guid teacherClassSubjectFileTypeId, string modifiedby , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @"UPDATE  TeacherClassSubjectFileType SET lastmodifiedby = @modifiedby ,  isdeleted = now() , islive = null  WHERE TeacherClassSubjectFileTypeId = @teacherClassSubjectFileTypeId 
                                
    AND TeacherClassSubjectFileTypeID Not IN ( Select TeacherClassSubjectFileTypeID FROM teacherClassSubjectFile where TeacherClassSubjectFileTypeId = @teacherClassSubjectFileTypeId AND  IsDeleted IS NULL)";
                    var id = connection.Execute(update, new
                    {
                        TeacherClassSubjectFileTypeID = teacherClassSubjectFileTypeId ,
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

        public TeacherClassSubjectFileType Save(TeacherClassSubjectFileType teacherClassSubjectFileType,string modifiedby , ref bool dbError)
        {

            var teacherClassSubjectFileTypeIsExist = GetTeacherClassSubjectFileTypeByCode(teacherClassSubjectFileType.Code, teacherClassSubjectFileType.SchoolId, ref dbError);
            if (dbError)
            {
                return null;
            }
            try
            {
                using (var connection = GetConnection())
                {
                    teacherClassSubjectFileType.Code = CleanIDcodeAlphanumeric(teacherClassSubjectFileType.Code);

               if (teacherClassSubjectFileTypeIsExist == null)
                {
                        teacherClassSubjectFileType.TeacherClassSubjectFileTypeId = Guid.NewGuid();
                        var insert = @"
                                INSERT INTO TeacherClassSubjectFileType
                                   (TeacherClassSubjectFileTypeID
                                   ,Description
                                   ,Code 
                                   ,SchoolID  
                                    ,CanParentAccessFile
                                    ,CanStudentAccessFile
                                    ,LastModifiedBy
                                    )
                             VALUES
                                   (
                                    @id , 
                                    @Description , 
                                    @code ,    
                                    @schoolID  ,
                                    @CanParentAccessFile ,
                                  @CanStudentAccessFile ,
                                    @modifiedby 
                                    ) 
                                ";
                        var id = connection.Execute(insert,
                                     new
                                     {
                                         id = teacherClassSubjectFileType.TeacherClassSubjectFileTypeId,
                                         Description = teacherClassSubjectFileType.Description,
                                         code = teacherClassSubjectFileType.Code, 
                                         schoolID = teacherClassSubjectFileType.SchoolId ,
                                         CanStudentAccessFile = teacherClassSubjectFileType.CanStudentAccessFile,
                                         CanParentAccessFile = teacherClassSubjectFileType.CanParentAccessFile,
                                         modifiedby = modifiedby
                                     });
                    }
                else
                {

                        var update = @" 
                                UPDATE TeacherClassSubjectFileType
					                            SET Description = @Description , lastmodifiedby = @modifiedby
                                                  ,CanStudentAccessFile = @CanStudentAccessFile 
                                                   ,CanParentAccessFile = @CanParentAccessFile
					                            WHERE TeacherClassSubjectFileTypeID = @id AND ISDELETED IS NULL
                                ";
                        var id = connection.Execute(update,
                                     new
                                     {
                                         id = teacherClassSubjectFileType.TeacherClassSubjectFileTypeId,
                                         Description = teacherClassSubjectFileType.Description,
                                         CanStudentAccessFile = teacherClassSubjectFileType.CanStudentAccessFile,
                                         CanParentAccessFile = teacherClassSubjectFileType.CanParentAccessFile,
                                         modifiedby = modifiedby
                                     });
                    }


                    return teacherClassSubjectFileType;

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
