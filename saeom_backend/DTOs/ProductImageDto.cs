namespace saeom_backend.DTOs
{
    public class ProductImageDto
    {
        public string Base64Image { get; set; } = null!;
        public bool IsPrimary { get; set; }
    }
}
