using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using iGrade.Domain;
using MySql.Data.MySqlClient;

namespace iGrade.Repository
{
    public class AppErrorLog 
    {

        private static string Connection = ConfigurationManager.AppSettings["LogConnection"];
        public static int Log(string AppName, string error)
        {
            try
            {
                var sql = @"INSERT INTO `log`( `AppName`, `Log`) VALUES ( @AppName , @error);select LAST_INSERT_ID();";

                using (var connection = new MySqlConnection(Connection))
                {
                   return  connection.ExecuteScalar<int>(sql, new { AppName = AppName, error = error });
                }
            }
            catch
            {
                return -1;
            }
        }
    
    }
}
