using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace iGrade.Repository
{
    public class SettingRepository : BaseRepository
    {
        public Setting GetSettingBySchoolID(Guid schoolID, ref bool dbFlag)
        {
            var sql = @"SELECT   se.* 
                        FROM Setting se
                        inner join Term  te on se.TermID = te.TermID
                        WHERE se.SchoolID = @SchoolID AND se.IsDeleted IS NULL  AND te.IsDeleted IS NULL ";

            using (var connection = GetConnection())
            {
                try
                {

                    var list = connection.Query<Setting>(sql, new { schoolID = schoolID }
                                    ).FirstOrDefault();

                    return list;
                }
                catch (Exception er)
                {
                    return null;
                }
            }

        }
        

        public bool UpdateSetting( Setting setting,string modifiedby , ref bool dbFlag)
        {

            using (var connection = GetConnection())
            {
                var update = @"UPDATE Setting
					            SET  AbsentFromSchoolEmailNotify = @absentFromSchoolEmailNotify ,    
					              AbsentFromSchoolSmsNotify = @absentFromSchoolSmsNotify  , 
                                   teacherNoticeBoard = @teacherNotice ,
                                   parentNotice = @parentNotice ,
                                    ExamMarkSubmissionClosingDate = @exam ,
                                    TestMarkSubmissionClosingDate = @test ,
                                     TermID = @termID    ,
                                    TermLastDateUpdated = now()    , 
                                  lastmodifiedby = @modifiedby
					            WHERE SchoolID = @SchoolID  AND ISDELETED IS NULL  ";
                var id = connection.Execute(update,
                             new
                             {
                                 absentFromSchoolEmailNotify = setting.AbsentFromSchoolEmailNotify,
                                 absentFromSchoolSmsNotify = setting.AbsentFromSchoolSmsNotify,
                                 teacherNotice = setting.TeacherNoticeBoard ,
                                 parentNotice = setting.ParentNotice ,
                                 SchoolID = setting.SchoolID,
                                 exam = setting.ExamMarkSubmissionClosingDate,
                                 test = setting.TestMarkSubmissionClosingDate,
                                 termID = setting.TermID,
                                 modifiedby = modifiedby
                             });

                return true;
            }
        } 
    }
}
