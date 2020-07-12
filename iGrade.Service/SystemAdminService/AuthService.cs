using iGrade.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.SystemAdminService
{
    public class AuthService
    {
        public AuthService()
        {

        }

        public bool Login(string username , string password , ref StringBuilder sbError)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }
            AuthRepository authRepository = new AuthRepository();
            var dbError = false;
            var isLogged = authRepository.SystemAdminLogIn(username, password, ref dbError);
            return isLogged;
        }
    }
}
