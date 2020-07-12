using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iGrade.Api.Controllers.ParentApi.Model
{
    public class LoginParent
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("schoolCode")]
        public string SchoolCode { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
