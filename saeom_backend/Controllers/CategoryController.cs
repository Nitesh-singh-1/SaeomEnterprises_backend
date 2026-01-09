using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using saeom_backend.DTOs;
using saeom_backend.Models;
using System.Text.Json;

namespace saeom_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("Addcategory")]
        public async Task<IActionResult> AddProductCategory(
         [FromBody] CreateProductCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                return Ok(new ApiResponse<object>
                {
                    response_code = 0,
                    response_message = "Bad Request Category Name is Required !"
                });
            }

            var exists = await _context.TblProductcategories
                .AnyAsync(c => c.CategoryName == dto.CategoryName);

            if (exists)
            {
                return Ok(new ApiResponse<object>
                {
                    response_code = 0,
                    response_message = "Category Already Exists"
                });
            }

            var category = new TblProductcategory
            {
                CategoryName = dto.CategoryName,
                IsActive = true,
                CreatedOn = DateTime.UtcNow
            };

            _context.TblProductcategories.Add(category);
            await _context.SaveChangesAsync();

            //return Ok(new
            //{
            //    message = "Category added successfully",
            //    categoryId = category.CategoryId
            //});
            return Ok(new ApiResponse<object>
            {
                response_code = 1,
                response_message = "success",
                data = new
                {
                    message = "Category added successfully",
                    categoryId = category.CategoryId

                }
            });
        }

        //updateProductCategory

        [HttpPut("category/UpdateCategory")]
        public async Task<IActionResult> UpdateProductCategory(
        [FromBody] UpdateProductCategoryDto dto)
        {
            if (dto.categoryID <= 0)
            {

                return Ok(new ApiResponse<object>
                {
                    response_code = 0,
                    response_message = "fail"
                });
            }
            var category = await _context.TblProductcategories
                .FirstOrDefaultAsync(c => c.CategoryId == dto.categoryID);

            if (category == null)
            {

                return Ok(new ApiResponse<object>
                {
                    response_code = 0,
                    response_message = "fail"
                });
            }

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                // Optional: check duplicate name
                var nameExists = await _context.TblProductcategories
                    .AnyAsync(c =>
                        c.CategoryName == dto.CategoryName &&
                        c.CategoryId != dto.categoryID);

                if (nameExists)
                {

                    return Ok(new ApiResponse<object>
                    {
                        response_code = 0,
                        response_message = "Name Already Exist!"
                    });
                }

                category.CategoryName = dto.CategoryName;
            }

            if (dto.IsActive.HasValue)
            {
                category.IsActive = dto.IsActive.Value;
            }

            category.ModifiedOn = DateTime.UtcNow; // if column exists

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                response_code = 1,
                response_message = "success",
                data = new
                {
                    message = "Category updated successfully",
                    categoryID = category.CategoryId

                }
            });
        }

        [AllowAnonymous]
        [HttpPost("getcategory")]
        public async Task<IActionResult> GetCategories(
        [FromBody] JsonElement body)
        {
            var query = _context.TblProductcategories.AsQueryable();

            if (body.TryGetProperty("categoryStatus", out JsonElement typeElement))
            {
                var type = typeElement.GetString();

                if (type == "actv")
                    query = query.Where(c => c.IsActive);

                else if (type == "dactv")
                    query = query.Where(c => !c.IsActive);
            }

            var categories = await query
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    c.IsActive
                })
                .ToListAsync();

            return Ok(categories);
        }
        [AllowAnonymous]
        [HttpPost("getcategorybyID")]
        public async Task<IActionResult> GetCategoryByID(
    [FromBody] JsonElement body)
        {
            // Validate JSON property
            if (!body.TryGetProperty("categoryID", out JsonElement idElement) ||
                idElement.ValueKind != JsonValueKind.Number)
            {
                return BadRequest("categoryID is required and must be a number.");
            }

            int categoryId = idElement.GetInt32();

            var category = await _context.TblProductcategories
                .Where(c => c.CategoryId == categoryId)
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    c.IsActive
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound("Category not found.");

            return Ok(category);
        }
    }
}
