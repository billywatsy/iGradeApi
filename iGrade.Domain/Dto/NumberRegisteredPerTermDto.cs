using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class NumberRegisteredPerTermDto
    {
        public int NumberRegistered { get; set; } 
        public int Year { get; set; }  
        public int TermNumber { get; set; }  
        public DateTime StartDate { get; set; }  
    }
}
