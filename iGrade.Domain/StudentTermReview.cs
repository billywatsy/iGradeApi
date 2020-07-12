using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain
{
    public class StudentTermReview
    {
        [JsonProperty("studentTermReviewID")]
        public Guid? StudentTermReviewID { get; set; }
        [JsonProperty("studentTermRegisterID")]
        public Guid StudentTermRegisterID { get; set; }
        [JsonProperty("teacherID")]
        public Guid TeacherID { get; set; }
        [JsonProperty("isReviewGood")]
        public bool IsReviewGood { get; set; } 
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
        [JsonProperty("star5")]
        public int Star5 { get; set; }

        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }


        public StudentTermRegister StudentTermRegister { get; set; }
    }

    
}
