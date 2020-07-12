using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace iGrade.Repository
{
    public class SchoolGroupRepository : BaseRepository
    {
        public SchoolGroup GetSchoolGroupByCode(string code, ref bool dbFlag)
        {
            var sql = @"SELECT   * FROM SchoolGroup WHERE SchoolGroupCode = @code AND ISDeleted Is null";

            using (var connection = GetConnection())
            {
                var list = connection.Query<SchoolGroup>(sql  , new { code = code }).FirstOrDefault();
                return list;
            }
        }
        public SchoolGroup GetSchoolGroupBySchoolID(Guid schoolID, ref bool dbFlag)
        {
            var sql = @"SELECT   *   FROM SchoolGroup
                        INNER JOIN School on SchoolGroup.SchoolGroupID = School.SchoolGroupID
                        WHERE School.SchoolId = @schoolID AND ISDeleted Is null";
            using (var connection = GetConnection())
            {
                var list = connection.Query<SchoolGroup>(sql, new { schoolID = schoolID }).FirstOrDefault();
                return list;
            }
        }
    }
}
