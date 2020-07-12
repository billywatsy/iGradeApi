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
    public class AdminRepository : BaseRepository
    {
        public Admin GetByEmail(string email, ref bool dbFlag)
        {
            var sql = @"SELECT *  FROM Admin   Where Email = @email AND IsDeleted Is NULL ;  ";

            using (var connection = GetConnection())
            {
                try
                {
                    var list = connection.Query<Admin>(sql, new { email = email })
                                         .FirstOrDefault();
                    return list;
                }
                catch (Exception er)
                {
                    return null;
                }
            }
        }


        public Admin GetByUsername(string username, ref bool dbFlag)
        {
            var sql = @"SELECT * FROM Admin    Where username = @username  AND IsDeleted Is NULL   ;   ";

            using (var connection = GetConnection())
            {
                try
                {
                    var list = connection.Query<Admin>(sql, new { username = username })
                                         .FirstOrDefault();
                    return list;
                }
                catch (Exception er)
                {
                    return null;
                }
            }
        }

    }
}
