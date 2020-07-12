using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avo
{
    public class HelperString
    {
        // Generate a random number between two numbers    
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        // Generate a random string with a given size    
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        // Generate a random password    
        public static string RandomPassword(int length)
        {
            StringBuilder builder = new StringBuilder(); 
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, length));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        public static string GetRandomUniqueString(string alphanumeric)
        {
            StringBuilder value = new StringBuilder("");
            TimeSpan span = DateTime.Now.Subtract(new DateTime(DateTime.Now.Year , 12, 31));
            if (span.Days.ToString().Length == 1)
            {
                value.Append("00").Append(span.Days);
            }
            else if (span.Days.ToString().Length == 2)
            {
                value.Append("02").Append(span.Days);
            }
            else if (span.Days.ToString().Length == 3)
            {
                value.Append(span.Days);
            } 
            value.Append(RandomString(1, true));
            value.Append(DateTime.Now.ToString("YYYY"));
            value.Append(RandomString(1, false));
            value.Append(RandomNumber(1000, 9999)); 
            value.Append(RandomString(2, false));
            return value.ToString();
        }
    }
}
