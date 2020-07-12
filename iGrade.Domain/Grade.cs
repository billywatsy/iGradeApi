using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain
{
    public class Grade
    {
        [JsonProperty("gradeID")]
        public Guid? GradeID { get; set; }
        [JsonProperty("schoolID")]
        public Guid SchoolID { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }


        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }
    }
}
