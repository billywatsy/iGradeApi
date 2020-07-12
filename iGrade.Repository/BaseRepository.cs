using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using MySql.Data.MySqlClient;

namespace iGrade.Repository
{
    public class BaseRepository 
    {
       private static string Connection = ConfigurationManager.AppSettings["DatabaseConnection"];
      public string CleanIDcodeAlphanumeric(string value)
       {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            else
            {
              value = value.Trim();
            }
            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex("^a-zA-Z0-9-");
            value = rgx.Replace(value, "");
            return value;
       }
        public string HashPassword(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }
        protected IDbConnection GetConnection()
        {
            return GetStaticConnection();
        }
        protected static IDbConnection GetStaticConnection()
        {
            return new MySqlConnection(Connection);
        }
        protected List<T> GetList<T>(string sql, object parameters = null)
        {
            using (var connection = GetConnection())
            {
                return connection.Query<T>(sql, parameters).ToList();
            }
        }
    }
}
