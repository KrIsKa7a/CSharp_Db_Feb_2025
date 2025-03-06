namespace ProductShop.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;

    public class ImportProductDto
    {
        [Required]
        [JsonProperty(nameof(Name))]
        public string Name { get; set; } = null!;

        [Required]
        [JsonProperty(nameof(Price))]
        public string Price { get; set; } = null!;

        [Required]
        [JsonProperty(nameof(SellerId))]
        public string SellerId { get; set; } = null!;

        [JsonProperty(nameof(BuyerId))]
        public string? BuyerId { get; set; }
    }
}
