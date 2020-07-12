using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iGrade.Api.Controllers.TeacherUserApi.Model;
using iGrade.Core.TeacherUserService;
using iGrade.Domain;
using iGrade.Domain.Dto;
using iGrade.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGrade.Api.Controllers.TeacherUserApi
{
    [Route("api/s/[controller]")]
    [ApiController]
    public class CommitteMemberController : BaseUser
    { 
        private CommitteMemberService _committeMemberService; 
        private UowRepository _repository;
        private LoggedUser _user;
        private StringBuilder _sbError;

        public void Init()
        {
            _user = UserSession();
            _repository = new UowRepository();
            _committeMemberService = new CommitteMemberService(_user , _repository);
            _sbError = new StringBuilder("");
        }

        [HttpGet]
        public ActionResult<object> GetList()
        {
            try
            {
                Init();
                return _committeMemberService.GetListBySchoolId( ref _sbError);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet("{id}")]
        public ActionResult<object> Get(Guid? id)
        {
            try
            {
                Init();
                if (id != null && Guid.Empty != id)
                {
                    return _committeMemberService.GetCommitteMemberByID((Guid)id, ref _sbError);
                }
                return null;
            }
            catch(Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost]
        public ActionResult<object> Save([FromBody]CommitteMemberForm committeMemberForm)
        {
            try
            {
                Init();
                StringBuilder sbError = new StringBuilder("");

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return "Failed getting committe Member id" ;
                }
                else
                {
                    CommitteMember committeMember = new CommitteMember();
                     committeMember.SchoolID = _user.SchoolID;
                    committeMember.Fullname = committeMemberForm.Fullname;
                    committeMember.Phone = committeMemberForm.Phone;
                    committeMember.Email = committeMemberForm.Email;
                    committeMember.Title = committeMemberForm.Title;
                    
                    var isSaved = _committeMemberService.Save(committeMember, ref sbError);
                    if (isSaved == null)
                    {
                        Response.StatusCode = 400;
                        return "committeMember save failed " + sbError.ToString() ;
                    }
                    else
                    {
                        return Ok(new { id = isSaved .CommitteMemberID});
                    }

                }
            }
            catch (Exception er)
            {
                return Error(er);
            }
        }


        [HttpPost("Delete")]
        public ActionResult<object> Delete([FromBody] Model.DeleteID deleteID)
        {
            Init();
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return    "Committe Member does not exist"
                ;
            }
            else
            {

                if (deleteID.id == null)
                {
                    Response.StatusCode = 400;
                    return "Committe Member does not exist"
                    ;
                }
                StringBuilder sbError = new StringBuilder("");

                var isDeleted = _committeMemberService.Delete(deleteID.id, ref sbError);

                if (!isDeleted)
                {
                    Response.StatusCode = 400;
                    return   "Committe Member Delete failed"
                    ;
                }
                else
                {
                    Response.StatusCode = 200;
                    return (string)"Committe Member Deleted Successfully";
                }
            }

        }
    }
}