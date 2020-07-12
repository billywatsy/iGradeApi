namespace iGrade.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CommitteMember
    {
        [JsonProperty("committeMemberID")]
        public Guid? CommitteMemberID { get; set; }
        [JsonProperty("schoolID")]
        public Guid SchoolID { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("fullname")]
        public string Fullname { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }



        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
 
    }
}
