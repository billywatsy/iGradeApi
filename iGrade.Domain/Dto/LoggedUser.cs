using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class LoggedParentUser
    {
        public string Email { get; set; }
        public List<string> StudentIds { get; set; }
    }


    public class LoggedUser
    {
        public Guid TeacherID { get; set; }//"61705";
        public Guid? ClassID { get; set; } //= "60191";
        public Guid SchoolID { get; set; } //= "96013";
        public bool isAdmin { get; set; } // true;
        public string Username { get; set; }// "50452";
        public string SchoolName { get; set; } //= "96013";
        public string WebToken { get; set; }
        public DateTime LastLogin { get; set; }
        public Guid TermID { get; set; }// "50452";
        public string CurrentTermYear { get; set; }// "50452";
        public string CurrentTermNumber { get; set; }// "50452";
        public string TeacherFullname { get; set; }// "50452";
        public string SchoolCode { get; set; }// "50452";
        public string DbConnectionKey { get; set; }// "50452";

    } 

}
