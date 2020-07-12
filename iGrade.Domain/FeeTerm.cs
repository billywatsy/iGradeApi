namespace iGrade.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class FeeType
    {
        [JsonProperty("feeTypeID")]
        public Guid? FeeTypeID { get; set; }
        [JsonProperty("schoolID")]
        public Guid SchoolID { get; set; }
        [JsonProperty("code")] 
        public string Code { get; set; }
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
