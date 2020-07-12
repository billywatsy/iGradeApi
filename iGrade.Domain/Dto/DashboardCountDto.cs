using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class DashboardCountDto
    { 
        public int Teacher { get; set; }
        public int StudentTermRegister { get; set; }
        public int CommitteMember { get; set; }
        public int Subject { get; set; }
        public int Class { get; set; }
        public int Female { get; set; }
        public int Male { get; set; }
    }
}
