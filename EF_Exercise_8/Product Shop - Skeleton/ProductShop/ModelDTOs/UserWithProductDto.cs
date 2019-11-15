namespace ProductShop.ModelDTOs
{
    using Newtonsoft.Json;

    public class UserWithProductDto
    {
        [JsonProperty("firstName", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName { get; set; }

        [JsonProperty("lastName", NullValueHandling = NullValueHandling.Ignore)]
        public string LastName { get; set; }

        [JsonProperty("age", NullValueHandling = NullValueHandling.Ignore)]
        public int? Age { get; set; }

        [JsonProperty("soldProducts")]
        public SoldProductsForUserWith SoldProducts { get; set; }
    }
}