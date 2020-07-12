using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService.Common
{
    public static class ValidatorCommon
    {
        public static bool IsPhoneValid(this string phone , ref StringBuilder sbError)
        {
            if (string.IsNullOrEmpty(phone))
            {
                sbError.Append("No numbers found");
                return false;
            }
            if(phone.Length < 5 || phone.Length > 14)
            { 
                sbError.Append("phone should be within this 6-14 characters");
                return false;
            }

            try
            {
                var num = long.Parse(phone);

                if (num <= 0)
                {
                    sbError.Append("phone number is a negative number");
                    return false;
                }
            }
            catch
            {
                sbError.Append("phone number is not valid");
                return false;
            }
            return true;
        }

        public static bool IsValidEmail(this string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
