using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Form
{
    public  class SchoolCreateNewFORM
    {
        
        // SCHOOL INFOR
        public string SchoolName { get; set; }
        public bool IsSchoolActive { get; set; }
        public string SubscriptionID { get; set; }
        public bool IsPrimarySchool { get; set; }
   
        // SETTING
    
        public int MaximumTermPerYear { get; set; }
        
        // Teacher
        [Display(Name = "Username")]
        public string TeacherUsername { get; set; } 
        [Display(Name = "Email")]
        public string TeacherEmail { get; set; } 
        [Display(Name = "Teacher Fullname")]
        public string TeacherFullname { get; set; } 
        [Display(Name = "Teacher Phone")]
        public string TeacherPhone { get; set; }
    }
}
