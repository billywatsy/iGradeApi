using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text; 
using Dapper;
using iGrade.Domain.Form;
using iGrade.Domain.Dto;

namespace iGrade.Repository
{
    public class TeacherClassSubjectRepository : BaseRepository 
    {
        public TeacherClassSubjectDto GetTeacherClassSubjectByID(Guid teacherClassSubjectID, ref bool dbFlag)
        {
            var sql = @"SELECT  tcs.TeacherClassSubjectID,
                         tcs.ClassID,
                         tcs.SubjectID,
                         tcs.TeacherID,
                         cla.LevelID,
                         cla.SchoolID,
                         tcs.TermID,
                         lv.LevelName,
                         sub.SubjectCode,
                         sub.SubjectName,
                         cla.ClassName,
                         tea.TeacherFullname As TeacherName,
                         cla.GradeID
                         FROM TeacherClassSubject tcs
                         inner join Class cla on cla.ClassID = tcs.ClassID
                         inner join Level lv on cla.LevelID = lv.LevelID
                         inner join Subject sub on sub.SubjectId = tcs.SubjectId
                         inner join Teacher tea on tea.TeacherID = tcs.TeacherID
                         WHERE  tcs.TeacherClassSubjectID = @teacherClassSubjectID 
                         AND  tcs.IsDeleted IS NULL 
                         AND cla.IsDeleted IS NULL
                         AND lv.IsDeleted IS NULL
                         AND sub.IsDeleted IS NULL
                         AND tea.IsDeleted IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<TeacherClassSubjectDto>(sql, 
                                     new { teacherClassSubjectID = teacherClassSubjectID } 
                                ).FirstOrDefault();
                return list;
            }
        }
        public List<TeacherClassSubjectDto> GetListTeacherClassSubjectsByTeacherIDandTermID(Guid teacherID, Guid termID, ref bool dbFlag)
        {
            var sql = @"SELECT   tcs.TeacherClassSubjectID,
                         tcs.ClassID,
                         tcs.SubjectID,
                         tcs.TeacherID,
                         cla.LevelID,
                         cla.SchoolID,
                         tcs.TermID,
                         lv.LevelName,
                         sub.SubjectCode,
                         sub.SubjectName,
                         cla.ClassName,
                         tea.TeacherFullname As TeacherName,
                         cla.GradeID
                         FROM TeacherClassSubject tcs
                         inner join Class cla on cla.ClassID = tcs.ClassID
                         inner join Level lv on cla.LevelID = lv.LevelID
                         inner join Subject sub on sub.SubjectId = tcs.SubjectId
                         inner join Teacher tea on tea.TeacherID = tcs.TeacherID
                         WHERE  tcs.TeacherID = @teacherID
                         AND tcs.TermID = @termID
                        AND  tcs.IsDeleted IS NULL 
                         AND cla.IsDeleted IS NULL
                         AND lv.IsDeleted IS NULL
                         AND sub.IsDeleted IS NULL
                         AND tea.IsDeleted IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<TeacherClassSubjectDto>(sql
                            , new { teacherID = teacherID, termID = termID }
                                ).AsList();
                return list;
            }
        }



        public List<TeacherClassSubjectDto> GetListForHeadOfDepartmentForATerm(Guid teacherID, Guid termID,Guid schoolId , ref bool dbFlag)
        {
            var sql = @"SELECT   tcs.TeacherClassSubjectID,
                         tcs.ClassID,
                         tcs.SubjectID,
                         tcs.TeacherID,
                         cla.LevelID,
                         cla.SchoolID,
                         tcs.TermID,
                         lv.LevelName,
                         sub.SubjectCode,
                         sub.SubjectName,
                         cla.ClassName,
                         tea.TeacherFullname As TeacherName,
                         cla.GradeID
                         FROM TeacherClassSubject tcs
                         inner join Class cla on cla.ClassID = tcs.ClassID
                         inner join Level lv on cla.LevelID = lv.LevelID
                         inner join Subject sub on sub.SubjectId = tcs.SubjectId
                         inner join Teacher tea on tea.TeacherID = tcs.TeacherID
where tcs.TermID = @termID
AND tcs.SubjectID IN (
        Select SubjectID FROM subject sub
        INNER JOIN department dep on sub.DepartmentId = dep.DepartmentID
        INNER JOIN teacherdepartment td on td.DepartmentId = dep.DepartmentID
        Where  td.IsHeadOfDepartment = 1
         AND dep.SchoolID = @schoolId
         AND sub.SchoolID = @schoolId
         AND td.TeacherId = @teacherId
        AND td.IsDeleted IS NULL
        AND sub.IsDeleted IS NULL 
        AND dep.IsDeleted IS NULL 
)";

            using (var connection = GetConnection())
            {
                var list = connection.Query<TeacherClassSubjectDto>(sql
                            , new { teacherId = teacherID, schoolId = schoolId , termID = termID }
                                ).AsList();
                return list;
            }
        }



        public List<TeacherClassSubjectDto> GetListTeacherClassSubjectsBySchoolIDandTermID(Guid SchoolID, Guid TermID, ref bool dbFlag)
        {
            var sql = @"SELECT    tcs.TeacherClassSubjectID,
                         tcs.ClassID,
                         tcs.SubjectID,
                         tcs.TeacherID,
                         cla.LevelID,
                         cla.SchoolID,
                         tcs.TermID,
                         lv.LevelName,
                         sub.SubjectCode,
                         sub.SubjectName,
                         cla.ClassName,
                         tea.TeacherFullname As TeacherName,
                         cla.GradeID
                         FROM TeacherClassSubject tcs
                         inner join Class cla on cla.ClassID = tcs.ClassID
                         inner join Level lv on cla.LevelID = lv.LevelID
                         inner join Subject sub on sub.SubjectId = tcs.SubjectId
                         inner join Teacher tea on tea.TeacherID = tcs.TeacherID
                         WHERE  tea.SchoolID = @schoolID
                         AND lv.SchoolID = @schoolID
                         AND tcs.TermID = @termID
                         AND  tcs.IsDeleted IS NULL 
                         AND cla.IsDeleted IS NULL
                         AND lv.IsDeleted IS NULL
                         AND sub.IsDeleted IS NULL
                         AND tea.IsDeleted IS NULL"; 

            using (var connection = GetConnection())
            {
                try
                {
                    var list = connection.Query<TeacherClassSubjectDto>(sql
                           , new { schoolID = SchoolID, termID = TermID }
                               ).ToList();

                    return list;
                }
                catch(Exception er)
                {
                    return null;
                }
               
               // return new List<TeacherClassSubjectDto>();
            }
        }
        public List<TeacherClassSubjectDto> GetListTeacherClassSubjectsByClassIDandTermID(Guid classID, Guid termID, ref bool dbFlag)
        {
            var sql = @"SELECT    tcs.TeacherClassSubjectID,
                         tcs.ClassID,
                         tcs.SubjectID,
                         tcs.TeacherID,
                         cla.LevelID,
                         cla.SchoolID,
                         tcs.TermID,
                         lv.LevelName,
                         sub.SubjectCode,
                         sub.SubjectName,
                         cla.ClassName,
                         tea.TeacherFullname As TeacherName,
                         cla.GradeID
                         FROM TeacherClassSubject tcs
                         inner join Class cla on cla.ClassID = tcs.ClassID
                         inner join Level lv on cla.LevelID = lv.LevelID
                         inner join Subject sub on sub.SubjectId = tcs.SubjectId
                         inner join Teacher tea on tea.TeacherID = tcs.TeacherID
                         WHERE  tcs.TeacherID = @teacherID
                         AND tcs.ClassID = @classID
                         AND  tcs.IsDeleted IS NULL 
                         AND cla.IsDeleted IS NULL
                         AND lv.IsDeleted IS NULL
                         AND sub.IsDeleted IS NULL
                         AND tea.IsDeleted IS NULL"; 

            using (var connection = GetConnection())
            {
                var list = connection.Query<TeacherClassSubjectDto>(sql
                            , new { classID = classID, termID = termID }
                                ).AsList();
                return list;
            }
        }

        
        public bool SaveTeacherClassSubjectsBySubject(Guid teacherId , Guid classID , Guid subjectID , Guid termId , string modifiedby , ref bool dbFlag)
        { 
            using (var connection = GetConnection())
            {
                var update = @"UPDATE TeacherClassSubject
                                SET TeacherID = @TeacherID ,lastmodifiedby = @modifiedby 
                                WHERE
                                    SubjectID = @SubjectID
                                    AND ClassID = @ClassID
                                    AND TermID = @TermID AND ISDELETED IS NULL";
                var id = connection.Execute(update,
                             new
                             {
                                 teacherID = teacherId,
                                 classID = classID,
                                 TermID = termId ,
                                 SubjectID = subjectID , 
                                 modifiedby = modifiedby
                             });

                if(id == 0)
                {
                    
                    var insert = @"INSERT INTO TeacherClassSubject
                                   (TeacherClassSubjectID
                                   ,TeacherID
                                   ,SubjectID
                                   ,ClassID
                                   ,TermID , LastModifiedBy)
                             VALUES
                                   (@id
                                   ,@TeacherID
                                   ,@SubjectID
                                   ,@ClassID
                                   ,@TermID , 
                                    @modifiedby
		                           )"; 
                    var recordsAffected = connection.Execute(insert,
                             new
                             { 
                                 id = Guid.NewGuid() , 
                                 teacherID = teacherId,
                                 classID = classID,
                                 SubjectID = subjectID ,
                                 termId = termId , 
                                 modifiedby = modifiedby
                             });
                }

                return true;
            } 
        }
		
		public int SaveTeacherClassSubjectsBySubject(List<TeacherClassSubject> teacherClassSubjects , string modifiedby , ref bool dbFlag)
        {
            int numberSaved = 0;
            using (var connection = GetConnection())
            {
				foreach(var teacherClassSubject in teacherClassSubjects)
				{
				
                var update = @"UPDATE TeacherClassSubject
                                SET TeacherID = @TeacherID  , LastModifiedBy = @modifiedby
                                WHERE
                                    SubjectID = @SubjectID
                                    AND ClassID = @ClassID
                                    AND TermID = @TermID AND IsDeleted IS NULL";
                var id = connection.Execute(update,
                             new
                             {
                                 teacherID = teacherClassSubject.TeacherID,
                                 classID = teacherClassSubject.ClassID,
                                 TermID = teacherClassSubject.TermID ,
                                 SubjectID = teacherClassSubject.SubjectID
                             });

                if(id == 0)
                {
                    
                    var insert = @"INSERT INTO TeacherClassSubject
                                   (TeacherClassSubjectID
                                   ,TeacherID
                                   ,SubjectID
                                   ,ClassID
                                   ,TermID , 
                                   , LastModifiedBy )
                             VALUES
                                   (@id
                                   ,@TeacherID
                                   ,@SubjectID
                                   ,@ClassID
                                   ,@TermID , @modifiedby 
		                           )"; 
                    id = connection.Execute(insert,
                             new
                             {
                                 id = Guid.NewGuid(),
                                 teacherID = teacherClassSubject.TeacherID,
                                 classID = teacherClassSubject.ClassID,
                                 TermID = teacherClassSubject.TermID ,
                                 SubjectID = teacherClassSubject.SubjectID ,
                                 modifiedby = modifiedby
                             });
                }

                if(id >= 0)
                {
                        numberSaved++;
                }
                	
					
				}
            }
            return numberSaved;
        }
		
         
        public bool Delete(Guid Id,string modifiedby ,  ref bool dbError)
        {
            try
            { 
                using (var connection = GetConnection())
                {
                    var update = @" UPDATE TeacherClassSubject SET  isdeleted = now() , islive = null , lastmodifiedby = @modifiedby   WHERE TeacherClassSubjectID  = @Id
               AND ISDELETED IS NULL
               AND TeacherClassSubjectID Not IN (Select TeacherClassSubjectID FROM EXAM WHERE TeacherClassSubjectID = @Id AND IsDeleted IS NULL)
               AND TeacherClassSubjectID Not IN(Select TeacherClassSubjectID FROM TEST WHERE TeacherClassSubjectID = @Id AND IsDeleted IS NULL)";
                    var id = connection.Execute(update, new
                    {
                        Id = Id,
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
    }
}
