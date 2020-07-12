namespace iGrade.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    
    public class PagedList<T> 
    {
        [JsonProperty("totalRows")]
        public int TotalCount {get; set;}
        [JsonProperty("page")]
        public int Page {get; set; }
        [JsonProperty("size")]
        public int Size{ get; set; }
        [JsonProperty("totalPages")]
        public int TotalPages {
            get
            {
                if(TotalCount <= Size)
                {
                    return 1;
                }
                return (int)Math.Ceiling(this.TotalCount * (double)this.Size); 
            }
                              }
        [JsonProperty("data")]
        public List<T> PagedData { get; set; }
        
    }
}

 