namespace ProductShop.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;

    public class ImportCategoryProductDto
    {
        [Required]
        [JsonProperty(nameof(CategoryId))]
        public string CategoryId { get; set; } = null!;

        [Required]
        [JsonProperty(nameof(ProductId))]
        public string ProductId { get; set; } = null!;
    }
}
