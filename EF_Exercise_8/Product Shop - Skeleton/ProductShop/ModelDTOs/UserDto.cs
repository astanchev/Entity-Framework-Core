namespace ProductShop.ModelDTOs
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class UserDto
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("soldProducts")]
        public List<SoldProduct> SoldProducts { get; set; }
    }
}