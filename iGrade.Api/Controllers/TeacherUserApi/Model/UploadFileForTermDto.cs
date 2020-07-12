using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iGrade.Api.Controllers.TeacherUserApi.Model
{
    public class UploadFileForTermDto
    {
        public IFormFile FileTerm { get; set; }
        public Guid TeacherClassSubjectID { get; set; }
        public Guid TeacherClassSubjectFileTypeID { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
    }
}
