//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iGrade.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    
    public class School
    {
        [JsonProperty("schoolId")]
        public Guid SchoolID { get; set; }
        [JsonProperty("schoolName")]
        public string SchoolName { get; set; }
        [JsonProperty("isSchoolActive")]
        public bool IsSchoolActive { get; set; }
        [JsonProperty("schoolCode")]
        public string SchoolCode { get; set; }
        [JsonProperty("schoolGroupID")]
        public Guid? SchoolGroupID { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }


        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
    }
}
