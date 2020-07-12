using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService.Common
{
    public static class StringCommon
    {
        public static string RandomAlphanumericString(int length)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            const string pool = "abcdefghjklmnpqrstuvwxyz23456789";
            var builder = new System.Text.StringBuilder();

            for (var i = 0; i < length; i++)
            {
                var c = pool[random.Next(0, pool.Length)];
                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}
