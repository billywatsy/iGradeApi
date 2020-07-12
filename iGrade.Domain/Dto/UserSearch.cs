using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Domain.Dto
{
    public class UserSearch
    {
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("page")]
        public int Page { get; set; }
        [JsonProperty("q")]
        public string Q { get; set; }
    }
}
