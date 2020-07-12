using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Form
{
    public class StudentTermRegisterFORM
    { 
        public string RegNumber { get; set; } 
        public Guid? StudentTermRegisterID { get; set; }
        public Guid ClassID { get; set; }
        public Guid TermID { get; set; }
        public bool IsPhoneSent { get; set; }
        public bool IsEmailSent { get; set; }
        public bool IsAllowedSent { get; set; } 
    }
}
