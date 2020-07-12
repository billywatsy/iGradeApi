

namespace iGrade.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    
    public class TestMark
    {
        [JsonProperty("testID")]
        public Guid TestID { get; set; }
        [JsonProperty("studentTermRegisterID")]
        public Guid StudentTermRegisterID { get; set; }
        [JsonProperty("mark")]
        public byte Mark { get; set; }
        [JsonProperty("percentage")]
        public byte Percentage { get; set; }

        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }

        public StudentTermRegister StudentTermRegister { get; set; }
        public Test Test { get; set; }
    }
}
