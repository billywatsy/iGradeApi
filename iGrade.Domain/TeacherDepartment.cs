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
    using System.ComponentModel.DataAnnotations;

    public class TeacherDepartment
    {
        [JsonProperty("teacherDepartmentId")]
        public Guid TeacherDepartmentId { get; set; }
        [JsonProperty("teacherId")]
        public Guid TeacherId { get; set; }
        [JsonProperty("departmentId")]
        public Guid DepartmentId { get; set; }
        [JsonProperty("isHeadOfDepartment")]
        public bool IsHeadOfDepartment { get; set; } 

        

        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }

    }
}
