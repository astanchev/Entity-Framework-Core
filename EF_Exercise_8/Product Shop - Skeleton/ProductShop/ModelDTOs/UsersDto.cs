namespace ProductShop.ModelDTOs
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class UsersDto
    {
        [JsonProperty("usersCount")]
        public long UsersCount { get; set; }

        [JsonProperty("users")]
        public List<UserWithProductDto> Users { get; set; }
    }
}