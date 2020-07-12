using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain
{
    public class GradeMark
    {
        [JsonProperty("gradeMarkID")]
        public Guid? GradeMarkID { get; set; }
        [JsonProperty("gradeID")]
        public Guid GradeID { get; set; }
        [JsonProperty("fromMark")]
        public int FromMark { get; set; }
        [JsonProperty("toMark")]
        public int ToMark { get; set; }
        [JsonProperty("gradeValue")]
        public string GradeValue { get; set; }
        [JsonProperty("defaultComment")]
        public string DefaultComment { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }


        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
    }
}
