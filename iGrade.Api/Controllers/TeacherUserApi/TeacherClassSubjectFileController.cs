using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iGrade.Api.Controllers.TeacherUserApi.Model;
using iGrade.Core.TeacherUserService;
using iGrade.Domain;
using iGrade.Domain.Dto;
using iGrade.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGrade.Api.Controllers.TeacherUserApi
{
    [Route("api/s/[controller]")]
    [ApiController]
    public class TeacherClassSubjectFileController : BaseUser
    {
        private TeacherClassSubjectFileService _teacherClassSubjectFileService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;
        private string _rootFolder;

        public TeacherClassSubjectFileController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _rootFolder = hostingEnvironment.WebRootPath;
        }

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _teacherClassSubjectFileService = new TeacherClassSubjectFileService(_user, _repository);
            _sbError = new StringBuilder("");
        }

        

        [HttpGet("TeacherClassSubject")]
        public ActionResult<object> Get(Guid teacherClassSubjectId , Guid teacherClassSubjectFileTypeId)
        {
            try
            {
                Init();
                 return _teacherClassSubjectFileService.GetTeacherClassSubjectFilesByTeacherClassSubjectIdAndTeacherClassSubjectFileTypeId(teacherClassSubjectId , teacherClassSubjectFileTypeId);
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost] 
        public ActionResult<object> SaveFile([FromForm]UploadFileForTermDto fileUp)
        {
            
            try
            {
                Init();
                var formData = HttpContext?.Request?.Form;
                var files = HttpContext?.Request?.Form?.Files;

                if (fileUp?.FileTerm == null)
                {
                    Response.StatusCode = 400;
                    return "File is required";
                } 
                StringBuilder sbError = new StringBuilder("");
                string schoolCode = "";

                var savefile = _teacherClassSubjectFileService.PreSaveValidate(fileUp.TeacherClassSubjectID,
                                                                           fileUp.TeacherClassSubjectFileTypeID,
                                                                           fileUp.Description,
                                                                           fileUp.Title,
                                                                           ref schoolCode ,
                                                                           ref sbError);

                if(savefile == null || !string.IsNullOrEmpty(sbError.ToString()))
                {
                    Response.StatusCode = 400;
                    return  sbError.ToString();
                }
                int MaxContentLength = 20 * (1024 * 1024 * 1); //Size = 20 MB

                var httpRequest = fileUp?.FileTerm;

                if (httpRequest == null)
                {
                    Response.StatusCode = 400;
                    return "Failed getting file";
                }

                if (httpRequest.Length > MaxContentLength)
                {
                    var message = string.Format("");
                    Response.StatusCode = 400;
                    return "Please Upload a file upto 20 mb.";
                }
                else
                {

                    var ext = httpRequest.FileName.Substring(httpRequest.FileName.LastIndexOf('.'));
                    var extension = ext.ToLower();

                    var name = "FILE_" + Guid.NewGuid() + extension;

                    var schoolDirectory = Path.Combine(_rootFolder, "SchoolFiles", schoolCode);
                    
                    if (!Directory.Exists(schoolDirectory))
                        Directory.CreateDirectory(schoolDirectory);

                    string fileFullPath = Path.Combine(schoolDirectory, name);
                    using (var stream = new FileStream(fileFullPath, FileMode.Create))
                    {
                        httpRequest.CopyTo(stream);
                    }
                    savefile.FileSizeInBytes = httpRequest.Length;
                    savefile.Filename =  name;
                    savefile.FullUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"] + @"/File/" + schoolCode + @"/" + savefile.Filename;

                    var save = _teacherClassSubjectFileService.Save(savefile, ref sbError);

                    if(save != null)
                    {
                        return true;
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return "Failed to save";
                    }
                }
            }
            catch (Exception er)
            {
                Response.StatusCode = 400;
                return ToDetailedError(er);
            }




        }


        [HttpPost("ddd")]
        public ActionResult<object> Save([FromBody]TeacherClassSubjectFile teacherClassSubjectFile)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return (string)"Failed getting teacherClassSubjectFile id" ;
                }
                else
                {
                    var requestFile = Request.Form.Files?.FirstOrDefault();
                    

                    var isSaved = _teacherClassSubjectFileService.Save(teacherClassSubjectFile, ref sbError);
                    if (isSaved == null)
                    {
                        Response.StatusCode = 400;
                        return (string)"teacherClassSubjectFile save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return Ok(new { id = isSaved.TeacherClassSubjectFileId });
                    } 
                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }

        [HttpPost("Delete")]
        public ActionResult<object> DeleteTeacherClassSubjectFile([FromBody]Model.DeleteID deleteID)
        {
            try
            { 
                Init();
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "TeacherClassSubjectFile does not exist"  ;
                }
                else
                {
                    if (deleteID == null)
                    {
                        Response.StatusCode = 400;
                        return "TeacherClassSubjectFile does not exist" ;
                    }
                    StringBuilder sbError = new StringBuilder("");
                     
                    var isDeleted = _teacherClassSubjectFileService.Delete(deleteID.id, ref sbError);

                    if (!isDeleted)
                    {
                        Response.StatusCode = 400;
                        return  "TeacherClassSubjectFile Delete failed";
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return (string)"TeacherClassSubjectFile Deleted Successfully";
                    }
                } 
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }
    }
}