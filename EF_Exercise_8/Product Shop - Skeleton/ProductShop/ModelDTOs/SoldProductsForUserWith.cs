namespace ProductShop.ModelDTOs
{
    using System.Collections.Generic;
    using Models;
    using Newtonsoft.Json;

    public class SoldProductsForUserWith
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("products")]
        public List<ProductForSoldProducts> Products { get; set; }
    }
}