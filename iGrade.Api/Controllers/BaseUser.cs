using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using iGrade.Domain;
using iGrade.Domain.Dto;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace iGrade.Api.Controllers
{
   // [EnableCors("AllowAll")]
    public class BaseUser : ControllerBase
    {
        public int auth_status_code = 200;
        public string error_message = "";
        public LoggedUser UserSession()
        {
            LoggedUser user = null;
            var request = Request;
            if (request == null)
            {
                auth_status_code = 401;
                error_message = "No token found";
                throw new Exception("401");
            }

            request?.Headers?.TryGetValue("access_token", out Microsoft.Extensions.Primitives.StringValues access_token);
            string token = access_token.ToString();
            if (string.IsNullOrEmpty(token))
            {
                auth_status_code = 401;
                error_message = "No token found";
                throw new Exception("401");
            }
           
                // get from db
                System.Text.StringBuilder sbError = new System.Text.StringBuilder();
                Core.TeacherUserService.AuthService authService = new Core.TeacherUserService.AuthService();
                user = authService.LoginSessionbyToken(token, ref sbError);

                if (user != null)
                {
                    return user;
                }
                else
                {
                    auth_status_code = 401;
                    error_message = "token expired";
                    throw new Exception("401");
                }
        }
       
        public PagedList<T> ListToPage<T>(List<T> list , int pageSize , int pageNumber )
        {
            if(list == null)
            {
                return new PagedList<T>();
            }
            if (pageSize < 1) pageSize = 1;
            if (pageNumber < 1) pageNumber = 1;

            int startIndex = (pageNumber - 1) * pageSize;

            List<T> filtered = list.Skip(startIndex)?.Take(pageSize)?.ToList() ?? new List<T>();
 
            return new PagedList<T>()
            {
                TotalCount = list.Count() ,
                Page = pageNumber ,
                Size = pageSize, 
                PagedData = filtered
            };
        }
       
    
        public ActionResult Error(Exception er)
        {
            if(er != null)
            {
                if (er.Message == "401")
                {
                    Response.StatusCode = 401;
                    return new UnauthorizedResult();
                }
            }
            Response.StatusCode = 400; 

            var log = "Error: " + ToDetailedError(er);
           var refNumber = Repository.AppErrorLog.Log(System.Configuration.ConfigurationManager.AppSettings["AppName"]+":Teacher", log);

            return new BadRequestObjectResult($"An Error occured if the problem persit contact support reference number : {refNumber} ");
        }

        public string ToDetailedError(Exception e)
        {
            if (e == null)
            {
                return "No Error Found";
            }
            var messages = new List<string>();
            do
            {
                messages.Add("Message :" + e.Message + " Stack Trace :" + e.StackTrace);
                e = e.InnerException;
            }
            while (e != null);

            return string.Join(" - ", messages);
        }

    }
}