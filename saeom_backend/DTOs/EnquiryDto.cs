namespace saeom_backend.DTOs
{
    public class EnquiryDto
    {
        public string FullName { get; set; } = null!;
        public string ContactNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Company { get; set; } = null;
        public int ProductCategoryID { get; set; }

        public string QueryMade { get; set; } = null!;

    }
}
