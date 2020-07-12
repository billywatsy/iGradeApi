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
    
    public class Subscription
    {
        [JsonProperty("subscriptionID")]
        public string SubscriptionID { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("numberOfStudent")]
        public int NumberOfStudent { get; set; }
        [JsonProperty("numberOfTeachers")]
        public int NumberOfTeachers { get; set; }
        [JsonProperty("numberOfClasses")]
        public int NumberOfClasses { get; set; }
        [JsonProperty("numberOfTest")]
        public int NumberOfTest { get; set; }
        [JsonProperty("teacherClassSubjectLimit")]
        public int TeacherClassSubjectLimit { get; set; }
        [JsonProperty("parentAccess")]
        public bool ParentAccess { get; set; }

        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }
        public School School { get; set; }
    }
}