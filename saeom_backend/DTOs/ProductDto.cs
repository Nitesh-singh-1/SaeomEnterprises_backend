namespace saeom_backend.DTOs
{
    public class CreateProductDto
    {
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public List<IFormFile> Images { get; set; } = new();
        public int PrimaryImageIndex { get; set; } = 0;
    }

    public class GetProductDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public int PrimaryImageIndex { get; set; } = 0;
    }
}
