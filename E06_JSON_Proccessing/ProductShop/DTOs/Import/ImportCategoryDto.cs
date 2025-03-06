namespace ProductShop.DTOs.Import
{
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;

    public class ImportCategoryDto
    {
        [Required]
        [JsonProperty("name")]
        public string? Name { get; set; }
    }
}
