namespace ProductShop.ModelDTOs
{
    using Newtonsoft.Json;

    public class CategoryDto
    {
        [JsonProperty("category")]
        public string CategoryName { get; set; }

        [JsonProperty("productsCount")]
        public long ProductsCount { get; set; }

        [JsonProperty("averagePrice")]
        public string AveragePrice { get; set; }

        [JsonProperty("totalRevenue")]
        public string TotalRevenue { get; set; }
    }
}