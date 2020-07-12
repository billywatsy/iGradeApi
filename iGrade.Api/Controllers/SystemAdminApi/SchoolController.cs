using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iGrade.Core.SystemAdminService;
using iGrade.Domain;
using iGrade.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGrade.Api.Controllers
{
    [Route("api/a/[controller]")]
    [ApiController]
    public class SchoolController : ControllerBase
    {
        SchoolService _schoolService;
        UowRepository _uowRepository;
        public SchoolController()
        {
            _uowRepository = new UowRepository();
            _schoolService = new SchoolService(_uowRepository);
        }

        [HttpGet]
        public ActionResult<IEnumerable<School>> Get()
        { 
            return _schoolService.GetList();
        }

    }

}