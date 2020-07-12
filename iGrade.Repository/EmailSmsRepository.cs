using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace iGrade.Repository
{
    public class EmailSmsRepository : BaseRepository
    { 
        public EmailSms GetEmailSmsByID(Guid schoolID, ref bool dbFlag)
        {
            var sql = @"SELECT   *
                         FROM EmailSms
                         WHERE SchoolID = @schoolID 
                           AND IsDeleted IS NULL ";

            using (var connection = GetConnection())
            {
                var list = connection.Query<EmailSms>(sql,  new { schoolID = schoolID } 
                                ).FirstOrDefault();
                return list;
            }
        }
         
    }
}
