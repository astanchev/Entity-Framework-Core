namespace ProductShop.ModelDTOs
{
    using Newtonsoft.Json;

    public class ProductForSoldProducts
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}