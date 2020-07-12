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
    public class TeacherClassSubjectFileRepository : BaseRepository
    {

         

        public List<TeacherClassSubjectFile> GetListByTeacherClassSubjectId(Guid TeacherClassSubjectId, ref bool dbFlag)
        {
            var sql = @"SELECT  TeacherClassSubjectFile.* ,teacherclasssubjectfiletype.Description As filetype ,subject.SubjectName
                        FROM TeacherClassSubjectFile
                        INNER JOIN teacherclasssubjectfiletype on TeacherClassSubjectFile.TeacherClassSubjectFileTypeID = teacherclasssubjectfiletype.TeacherClassSubjectFileTypeID
                        INNER JOIN TeacherClassSubject on TeacherClassSubject.TeacherClassSubjectID = teacherclasssubjectfile.TeacherClassSubjectID
                        INNER JOIN subject on subject.subjectID = TeacherClassSubject.subjectID
                        where TeacherClassSubjectFile.TeacherClassSubjectId = @TeacherClassSubjectId
                        AND TeacherClassSubjectFile.ISDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<TeacherClassSubjectFile>(sql,
                                                new
                                                {
                                                    TeacherClassSubjectId = TeacherClassSubjectId
                                                }).AsList();
                return list;
            }
        }

        public List<TeacherClassSubjectFileDto> GetListByTeacherClassSubjectIdAndTeacherClassSubjectFileTypeId(Guid TeacherClassSubjectId, Guid TeacherClassSubjectFileTypeId, ref bool dbFlag)
        {
            var sql = @"SELECT  TeacherClassSubjectFile.* ,teacherclasssubjectfiletype.Description As filetype ,subject.SubjectName
                        FROM TeacherClassSubjectFile
                        INNER JOIN teacherclasssubjectfiletype on TeacherClassSubjectFile.TeacherClassSubjectFileTypeID = teacherclasssubjectfiletype.TeacherClassSubjectFileTypeID
                        INNER JOIN TeacherClassSubject on TeacherClassSubject.TeacherClassSubjectID = teacherclasssubjectfile.TeacherClassSubjectID
                        INNER JOIN subject on subject.subjectID = TeacherClassSubject.subjectID
                        where TeacherClassSubjectFile.TeacherClassSubjectId = @TeacherClassSubjectId 
                        AND TeacherClassSubjectFile.TeacherClassSubjectFileTypeId = @TeacherClassSubjectFileTypeId
                        AND TeacherClassSubjectFile.ISDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<TeacherClassSubjectFileDto>(sql,
                                                new
                                                {
                                                    TeacherClassSubjectId = TeacherClassSubjectId,
                                                    TeacherClassSubjectFileTypeId = TeacherClassSubjectFileTypeId
                                                }).AsList();
                return list;
            }
        }


        public List<TeacherClassSubjectFileDto> GetListByTermIdAndClassId(Guid classId, Guid termId, ref bool dbFlag)
        {
            var sql = @"SELECT  TeacherClassSubjectFile.* ,teacherclasssubjectfiletype.Description As filetype ,subject.SubjectName
                        FROM TeacherClassSubjectFile
                        INNER JOIN teacherclasssubjectfiletype on TeacherClassSubjectFile.TeacherClassSubjectFileTypeID = teacherclasssubjectfiletype.TeacherClassSubjectFileTypeID
                        INNER JOIN TeacherClassSubject on TeacherClassSubject.TeacherClassSubjectID = teacherclasssubjectfile.TeacherClassSubjectID
                        INNER JOIN subject on subject.subjectID = TeacherClassSubject.subjectID
                        where TeacherClassSubject.TermId = @termId 
                        AND TeacherClassSubject.ClassId = @classId
                        AND TeacherClassSubjectFile.ISDELETED IS NULL
                        ORDER BY `teacherclasssubjectfile`.`CreatedDate` DESC";

            using (var connection = GetConnection())
            {
                var list = connection.Query<TeacherClassSubjectFileDto>(sql,
                                                new
                                                {
                                                    classId = classId,
                                                    termId = termId
                                                }).AsList();
                return list;
            }
        }


        public TeacherClassSubjectFileDto GetTeacherClassSubjectFile(Guid teacherClassSubjectFileId, ref bool dbFlag)
        {
            var sql = @"SELECT  TeacherClassSubjectFile.* ,teacherclasssubjectfiletype.Description As filetype ,subject.SubjectName
                        FROM TeacherClassSubjectFile
                        INNER JOIN teacherclasssubjectfiletype on TeacherClassSubjectFile.TeacherClassSubjectFileTypeID = teacherclasssubjectfiletype.TeacherClassSubjectFileTypeID
                        INNER JOIN TeacherClassSubject on TeacherClassSubject.TeacherClassSubjectID = teacherclasssubjectfile.TeacherClassSubjectID
                        INNER JOIN subject on subject.subjectID = TeacherClassSubject.subjectID
                        where TeacherClassSubjectFile.teacherClassSubjectFileId = @teacherClassSubjectFileId 
                        AND TeacherClassSubjectFile.ISDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<TeacherClassSubjectFileDto>(sql,
                                                new
                                                {
                                                    teacherClassSubjectFileId = teacherClassSubjectFileId
                                                }).FirstOrDefault();
                return list;
            }
        }
        

        public bool Delete(Guid teacherClassSubjectFileId, string modifiedby , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var update = @"UPDATE  TeacherClassSubjectFile SET lastmodifiedby = @modifiedby ,  isdeleted = now() , islive = null  WHERE TeacherClassSubjectFileId = @teacherClassSubjectFileId 
                                ";
                    var id = connection.Execute(update, new
                    {
                        teacherClassSubjectFileId = teacherClassSubjectFileId,
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

        public TeacherClassSubjectFile Insert(TeacherClassSubjectFile teacherClassSubjectFile, string modifiedby , ref bool dbError)
        {
            try
            {
                using (var connection = GetConnection())
                {  
                        teacherClassSubjectFile.TeacherClassSubjectFileId = Guid.NewGuid();
                        var insert = @"
                                INSERT INTO TeacherClassSubjectFile
                                   (TeacherClassSubjectFileId
                                   ,TeacherClassSubjectID
                                   ,TeacherClassSubjectFileTypeID 
                                   ,Title  
                                    ,Description
                                    ,FileSizeInBytes
                                    ,Filename
                                    ,FullUrl
                                    ,LastModifiedBy
                                    )
                             VALUES
                                   (@TeacherClassSubjectFileId ,
                                    @TeacherClassSubjectID , 
                                    @TeacherClassSubjectFileTypeID ,    
                                    @Title  ,
                                    @Description ,
                                    @FileSizeInBytes , 
                                    @Filename , 
                                    @FullUrl ,
                                    @modifiedby 
                                    ) 
                                ";
                        var id = connection.Execute(insert,
                                     new
                                     {
                                         TeacherClassSubjectFileId = teacherClassSubjectFile.TeacherClassSubjectFileId ,
                                         TeacherClassSubjectID = teacherClassSubjectFile.TeacherClassSubjectId,
                                         TeacherClassSubjectFileTypeID = teacherClassSubjectFile.TeacherClassSubjectFileTypeId,
                                         Title = teacherClassSubjectFile.Title,
                                         Description = teacherClassSubjectFile.Description,
                                         FileSizeInBytes = teacherClassSubjectFile.FileSizeInBytes,
                                         Filename = teacherClassSubjectFile.Filename,
                                         FullUrl = teacherClassSubjectFile.FullUrl ,
                                         modifiedby = modifiedby
                                     });

                    return teacherClassSubjectFile;

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
