using iGrade.Core.TeacherUserService.Common;
using iGrade.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.ParentService
{
    public class AuthService
    {
        Repository.UowRepository _uowRepository;
        public AuthService()
        {
            _uowRepository = new Repository.UowRepository();
        }
        public ParentSessionDto Login(string username, string schoolCode, string password, ref StringBuilder sbError)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || password.Length > 200 || username.Length > 40)
                {
                    return null;
                }

                bool dbFlag = false;
                var teacher = _uowRepository.ParentRepository.Login(username, schoolCode, password, ref dbFlag);

                if (teacher != null)
                {
                    return new ParentSessionDto()
                    {
                        Token = teacher.WebToken
                    };
                }

                sbError.Append("Wrong username and password ");
                return null;
            }
            catch (Exception er)
            {
                sbError.Append("Wrong username and password ");
            }
            return null;
        }


        public bool Register(string username, string schoolCode, ref StringBuilder sbError)
        {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(schoolCode) || schoolCode.Length > 200 || username.Length > 40)
                {
                    return false;
                }

                bool dbFlag = false;

                var parent = _uowRepository.ParentRepository.IsSchoolCodeAndEmailExist(schoolCode, username, ref dbFlag);

                if (parent)
                {
                    return true;
                }
                else
                {
                    var isschoolExist = _uowRepository.SchoolRepository.GetSchoolByCode(schoolCode, ref dbFlag);
                    if (isschoolExist == null)
                    {
                        sbError.Append("School code does not exist");
                        return false;
                    }

                    var password = StringCommon.RandomAlphanumericString(5);

                    var save = _uowRepository.ParentRepository.Register(username, schoolCode, password, ref dbFlag);

                    if (save)
                    {
                        var message = $"Hi   <br/> Your account has now been setup  " +
                            $"<br> Password : " + password;
                        var sendEmail = iGrade.Core.TeacherUserService.Common.Email.SendEmail(username, " New Account ", message, ref sbError);

                        return true;
                    }
                    else
                    {
                        sbError.Append("Error creating account if the eror persist try contancting support");
                        return false;
                    }
                }

            return false;
        }



        public bool SendMeOneTimePin(string username, string schoolCode, ref StringBuilder sbError)
        {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(schoolCode) || schoolCode.Length > 200 || username.Length > 40)
                {
                    return false;
                }

                bool dbFlag = false;

                var parent = _uowRepository.ParentRepository.IsSchoolCodeAndEmailExist(username ,schoolCode, ref dbFlag);

                if (!parent)
                {
                    sbError.Append("email and school code does not exist , try registering your account");
                    return false;
                }
                else
                {
                    var isschoolExist = _uowRepository.SchoolRepository.GetSchoolByCode(schoolCode, ref dbFlag);
                    if (isschoolExist == null)
                    {
                        sbError.Append("School code does not exist");
                        return false;
                    }

                    var password = StringCommon.RandomAlphanumericString(5);

                    var save = _uowRepository.ParentRepository.UpdateOneTimePin(username, schoolCode, password, ref dbFlag);

                    if (save)
                    {
                        var message = $"Hi   <br/> Please user the below one time pin  " +
                            $"<br> Password : " + password;
                        var sendEmail = iGrade.Core.TeacherUserService.Common.Email.SendEmail(username, " New Account ", message, ref sbError);

                        return true;
                    }
                    else
                    {
                        sbError.Append("Error creating account if the eror persist try contancting support");
                        return false;
                    }
                }

            
            return false;
        }



        public Domain.Dto.ParentSessionDto LoginSessionbyToken(string token, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var user = _uowRepository.ParentRepository.GetByWebToken(token, ref dbFlag);

            if (user == null)
            {
                sbError.Append("session expired");
                return null;
            } 
            return new ParentSessionDto()
            {
                Email = user.ParentEmail , 
                SchoolCode = user.SchoolCode , 
                Token = user.WebToken 
            };
        }
    }
}
