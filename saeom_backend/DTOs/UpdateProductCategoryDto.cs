namespace saeom_backend.DTOs
{
    public class UpdateProductCategoryDto
    {
        public int categoryID { get; set; }
        public string? CategoryName { get; set; }
        public bool? IsActive { get; set; }
    }
}
