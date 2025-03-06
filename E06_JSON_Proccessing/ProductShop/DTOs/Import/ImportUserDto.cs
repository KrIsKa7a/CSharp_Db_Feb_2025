namespace ProductShop.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;

    public class ImportUserDto
    {
        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        [Required]
        [JsonProperty("lastName")]
        public string LastName { get; set; } = null!;

        [JsonProperty("age")]
        public string? Age { get; set; }
    }
}
