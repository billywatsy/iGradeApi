using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using iGrade.Domain;
using iGrade.Domain.Dto;
using Microsoft.AspNetCore.Mvc;


namespace iGrade.Api.Controllers
{
    public class BaseParent : ControllerBase
    {

        public int auth_status_code = 200;
        public string error_message = "";
        public ParentSessionDto UserSession()
        {
            ParentSessionDto user = null;
            var request = Request;
            if (request == null)
            {
                auth_status_code = 401;
                error_message = "No token found";
                throw new Exception("401");
            }

            request?.Headers?.TryGetValue("token", out Microsoft.Extensions.Primitives.StringValues token_header);
            request?.Headers?.TryGetValue("school_code", out Microsoft.Extensions.Primitives.StringValues school_code_header);
            string token = token_header.ToString();
            string school_code = school_code_header.ToString();
            if (string.IsNullOrEmpty(school_code) || string.IsNullOrEmpty(token))
            {
                auth_status_code = 401;
                error_message = "No token found";
                throw new Exception("401");
            } 
                // get from db
                Repository.ParentRepository parent = new Repository.ParentRepository();
                System.Text.StringBuilder sbError = new System.Text.StringBuilder();

                bool dbError = false;
                var studentData = parent.GetByWebToken(token, ref dbError); 

                if (studentData != null)
                {
                    return new ParentSessionDto() { 
                    Email = studentData.ParentEmail , 
                    SchoolCode = studentData.SchoolCode , 
                    Token = token
                    };
                }
                else
                {
                    auth_status_code = 401;
                    error_message = "Error authorizing pleasee try again";
                    throw new Exception("401");
                }
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
            try
            {
                Response.StatusCode = 400;
                var log = "Error: " + ToDetailedError(er);
                var refNumber = Repository.AppErrorLog.Log(System.Configuration.ConfigurationManager.AppSettings["AppName"] + ":Parent", log);

                return new BadRequestObjectResult($"An Error occured if the problem persit contact support : ref number {refNumber} ");

            }
            catch
            {
                return new BadRequestObjectResult($"An Error occured if the problem persit contact support ");
            }


        }
    }
}