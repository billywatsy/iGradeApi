using iGrade.Core.TeacherUserService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class AuthService
    {
        Repository.AuthRepository _authRepository;
        Repository.UowRepository _uowRepository;
        public AuthService()
        {
            _authRepository = new Repository.AuthRepository();
            _uowRepository = new Repository.UowRepository();
        }
        public Domain.Dto.LoggedUser Login(string username, string password, ref StringBuilder sbError)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || password.Length > 200 || username.Length > 40)
                {
                    return null;
                }

                bool dbFlag = false;
                var teacher = _authRepository.Login(username, password, ref dbFlag);
                
                if (teacher == null)
                {
                    teacher = _authRepository.LoginOneTimePin(username, password, ref dbFlag);

                    if (teacher == null)
                    {
                        sbError.Append("Wrong username and password");
                        return null;
                    }
                }
                if (!teacher.IsActive)
                {
                    sbError.Append("Account has been deactivated . Please contact school admin to access");
                    return null;
                }

                var sessionLogged = _authRepository.SessionSetting((Guid)teacher.TeacherID, ref dbFlag);
                if (sessionLogged == null)
                {
                    sbError.Append("Failed getting school settings details . please contact service provider if the error persist");
                    return null;
                }

                _authRepository.UpdateLastLoginDateAndToken(sessionLogged.TeacherID, sessionLogged.WebToken, ref dbFlag);
                
                Repository.LogRepository.SaveActivity(new Domain.Log()
                {
                    TeacherID = sessionLogged.TeacherID , 
                    EntityType = "Login" , 
                    ActionDate = DateTime.Now.AddMilliseconds(1) ,
                    ActionDetails = "Logged In" , 
                    ActionType = "Login"
                }, ref dbFlag);


                return sessionLogged;
            }
            catch (Exception er)
            {
                sbError.Append("Wrong username and password ");
            }
            return null;
        }

        public Domain.Dto.LoggedUser LoginSessionbyToken(string token, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var user = _authRepository.SessionSettingByToken(token, ref dbFlag);

            if (user == null) return null;

            if(
                user.LastLogin.Year == DateTime.Now.Year &&
                user.LastLogin.Month == DateTime.Now.Month &&
                user.LastLogin.Day == DateTime.Now.Day
                                )
            {
                return user;
            }
            _authRepository.UpdateLastLoginDateAndToken(user.TeacherID, StringCommon.RandomAlphanumericString(90), ref dbFlag);
            return null;
        }
        public Domain.Dto.LoggedUser LoginSession(Guid teacherID, ref StringBuilder sbError)
        {
            bool dbFlag = false;
          var sessionLogged = _authRepository.SessionSetting(teacherID, ref dbFlag);
                if (sessionLogged == null)
                {
                    sbError.Append("Failed getting school settings details . please contact service provider if the error persist");
                    return null;
                }
                return sessionLogged;    
        }

        public bool ChangePassword(string username , string currentOldPassword, string newPassword, string confirmPassword , ref StringBuilder sbError)
        { 
            var isLogged = Login(username, currentOldPassword, ref sbError);
            if(isLogged == null)
            {
                sbError.Append("Wrong old password");
                return false;
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                sbError.Append("Fill in new password");
                return false;
            }

            if (!newPassword.Equals(confirmPassword))
            {
                sbError.Append("new password mismatch");
                return false;
            }

            if (newPassword.Length <= 6)
            {
                sbError.Append("password should be greater than 6 characters");
                return false;
            }
            if (newPassword.Length >= 14)
            {
                sbError.Append("password should be less than 14 characters");
                return false;
            }


            if(newPassword.ToLower() == isLogged.TeacherFullname)
            {
                sbError.Append("passsword should not be equal to your name");
                return false;
            }

            if(decimal.TryParse(newPassword , out decimal test))
            {
                sbError.Append("passsword should not a number");
                return false;
            }

            bool dbFlag = false;
            var isChanged = _authRepository.PasswordChange(isLogged.TeacherID, newPassword , Guid.NewGuid().ToString() , ref dbFlag);

            if (isChanged)
            {
                return true;
            }
            sbError.Append("Password change failed");
            return false;
        }

        public bool ChangePasswordForgot(string token ,string password , string newPassword, ref StringBuilder sbError)
        {
            bool dbError = false;

            var auth = _authRepository.GetAuthByForgotPassword(token, ref dbError);

            if(auth == null)
            {
                sbError.Append("token does not exist");
                return false;
            }
            var teacher = _uowRepository.TeacherRepository.GetTeacherById(auth.TeacherID, ref dbError);
            if (teacher == null)
            {
                sbError.Append("User does not exist");
                return false;
            }
            
            if(
                (auth.CreatedDate.Year != DateTime.Today.Year) &&
                (auth.CreatedDate.Month != DateTime.Today.Month) &&
                (auth.CreatedDate.Day != DateTime.Today.Day)
               )
            {
                sbError.Append("Verfication code was not generated today");
                return false;
            }
             
            if(password != newPassword)
            {
                sbError.Append("Password mismatch");
                return false;
            }

            if (newPassword.Length <= 6)
            {
                sbError.Append("password should be greater than 6 characters");
                return false;
            }
            if (newPassword.Length >= 14)
            {
                sbError.Append("password should be less than 14 characters");
                return false;
            }


            if (newPassword.ToLower() == teacher.TeacherFullname)
            {
                sbError.Append("passsword should not be equal to your name");
                return false;
            }

            if (decimal.TryParse(newPassword, out decimal test))
            {
                sbError.Append("passsword should not a number");
                return false;
            }

            bool dbFlag = false;
            var isChanged = _authRepository.PasswordChange((Guid)teacher.TeacherID, newPassword, Guid.NewGuid().ToString() , ref dbFlag);

            if (isChanged)
            {
                return true;
            }
            sbError.Append("Password change failed");
            return false;
        }

        public bool RequestNewForgotPassword(string email ,string username , ref StringBuilder sbError)
        {
            if (string.IsNullOrEmpty(email))
            {
                sbError.Append("Email does not exist for user");
                return false ;
            }

            if (string.IsNullOrEmpty(username))
            {
                sbError.Append("username does not exist");
                return false;
            }

        
    bool dbFlag = false;
            var isUserExist = _uowRepository.TeacherRepository.GetTeacherByEmail(email, ref dbFlag);

            if(isUserExist == null)
            {
                sbError.Append("Email does not exist");
                return false;
            }

            if(username.ToLower() != isUserExist.TeacherUsername.ToLower())
            {
                sbError.Append("wrong account username and email");
                return false;
            }

            var onetimepin = StringCommon.RandomAlphanumericString(6);
            var setNewCode = _authRepository.UpdateOneTimePin((Guid)isUserExist.TeacherID, onetimepin  , ref dbFlag);

            if (setNewCode)
            {
                var message = $"Hi  {isUserExist.TeacherFullname} <br/> Please use the following One Time Pin valid for today <br/> <center><strong>Pin:</strong>{onetimepin}<center>  <br/> <small>Please make sure to change your password after logging in </small>";

                var sendEmail = iGrade.Core.TeacherUserService.Common.Email.SendEmail(email, "Forgot Password", message, ref sbError);

                return sendEmail;
            }
            sbError.Append("Failed getting code please try again");
            return false ;
        }

        public bool ForgotPasswordTokenVerify(string forgotPasswordToken, ref StringBuilder sbError)
        {
            bool dbError = false;
            if (string.IsNullOrEmpty(forgotPasswordToken))
            {
                sbError.Append("Failed getting forgot password token");
                return false;
            }
            var token = _authRepository.GetAuthByForgotPassword(forgotPasswordToken, ref dbError);

            if (token == null)
            {
                sbError.Append("token does not exist");
                return false;
            }
            return true;
        }
    }
}
