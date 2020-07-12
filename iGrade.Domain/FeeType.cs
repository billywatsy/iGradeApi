namespace iGrade.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class FeeTerm
    {
        [JsonProperty("feeTermID")]
        public Guid? FeeTermID { get; set; }
        [JsonProperty("feeTypeID")]
        public Guid FeeTypeID { get; set; }
        [JsonProperty("termID")]
        public Guid TermID { get; set; }
        [JsonProperty("currencySymbol")]
        public string CurrencySymbol { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }


        [JsonProperty("isDeleted")]
        public System.DateTime? IsDeleted { get; set; }
        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        [JsonProperty("isLive")]
        public bool? IsLive { get; set; }

        public FeeType FeeType { get; set; } 
        public Term Term { get; set; } 
    }
}
