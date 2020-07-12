using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain
{
    public class SchoolInformation
    {
        [JsonProperty("schoolId")]
        public Guid? SchoolId { get; set; }
        [JsonProperty("roadName")]
        public string RoadName { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("lattitude")]
        public double? Lattitude  { get; set; }
        [JsonProperty("longitude")]
        public double? Longitude { get; set; }
        [JsonProperty("logoUrl")]
        public string LogoUrl  { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }


        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }

    }
}
