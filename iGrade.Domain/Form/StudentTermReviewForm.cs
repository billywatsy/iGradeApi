using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Domain.Form
{
    public class StudentTermReviewForm
    {
        [JsonProperty("regNumber")]
        public string RegNumber { get; set; }
        [JsonProperty("isReviewGood")]
        public bool IsReviewGood { get; set; } 
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("star5")]
        public int Star5 { get; set; }
    }
}
