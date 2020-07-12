using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Domain.Dto
{
    public class ExcelListResponseDto
    {
        [JsonProperty("success")]
        public List<string> Success { get; set; }
        [JsonProperty("error")]
        public List<string> Error { get; set; }
    }
}
