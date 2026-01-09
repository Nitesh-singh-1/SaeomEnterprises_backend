using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using saeom_backend.DTOs;
using saeom_backend.Models;

namespace saeom_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnquiryController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public EnquiryController(ApplicationDbContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        [HttpPost("AddEnquiry")]
        public async Task<IActionResult> AddEnquiry([FromBody] EnquiryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                return BadRequest("Name is required.");
            if (string.IsNullOrWhiteSpace(dto.ContactNumber))
                return BadRequest("Contact Number is required.");
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");
            if (dto.ProductCategoryID==null || dto.ProductCategoryID==0)
                return BadRequest("ProductCategoryID is required.");
            if (string.IsNullOrWhiteSpace(dto.QueryMade))
                return BadRequest("QueryDescription is required.");

            var enquiry = new TblContactu
            {
                FullName = dto.FullName,
                ContactNumber = dto.ContactNumber,
                Email = dto.Email,
                CompanyName = dto.Company,
                ProductCategoryId = dto.ProductCategoryID,
                QueryMade = dto.QueryMade,
                CreatedOn = DateTime.UtcNow
            };
            _context.TblContactus.Add(enquiry);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "Enquiry added successfully",
                EnquiryID = enquiry.ContactId
            });
        }
    }
}
