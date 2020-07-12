using Dapper;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Repository
{
    public class SubscriptionRepository : BaseRepository
    {
        public List<Subscription> GetList( ref bool dbFlag)
        {
            var sql = @"SELECT *   FROM Subscription  AND ISDELETED IS NULL";

            using (var connection = GetConnection())
            {
                var list = connection.Query<Subscription>(sql  )
                                 .AsList();
                return list;
            }
        }
    }
}
