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
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
       


        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddProduct([FromForm] CreateProductDto request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1️⃣ Validate category
                var categoryExists = await _context.TblProductcategories
                    .AnyAsync(c => c.CategoryId == request.CategoryId && c.IsActive);

                if (!categoryExists)
                    return BadRequest("Invalid or inactive category.");

                // 2️⃣ Create product
                var product = new TblProduct
                {
                    ProductName = request.ProductName,
                    ProductDescription = request.Description,
                    CategoryId = request.CategoryId,
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow
                };

                _context.TblProducts.Add(product);
                await _context.SaveChangesAsync(); // ProductId generated

                // 3️⃣ Create product folder
                var productFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "products",
                    product.ProductId.ToString()
                );

                Directory.CreateDirectory(productFolder);

                // 4️⃣ Save images
                for (int i = 0; i < request.Images.Count; i++)
                {
                    var image = request.Images[i];

                    if (image.Length == 0)
                        throw new Exception("Invalid image file.");

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                    var filePath = Path.Combine(productFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    _context.TblProductImages.Add(new TblProductImage
                    {
                        ProductId = product.ProductId,
                        ImagePath = $"uploads/products/{product.ProductId}/{fileName}",
                        IsPrimary = i == request.PrimaryImageIndex,
                        IsActive = true,
                        CreatedOn = DateTime.UtcNow
                    });
                }

                await _context.SaveChangesAsync();

                // 5️⃣ Commit transaction
                await transaction.CommitAsync();

                //return Ok(new
                //{
                //    message = "Product added successfully",
                //    productId = product.ProductId
                //});
                return Ok(new ApiResponse<object>
                {
                    response_code = 1,
                    response_message = "success",
                    data = new
                    {
                        message = "Product added successfully",
                        productId = product.ProductId

                    }
                });
            }
            catch (Exception ex)
            {
                // 6️⃣ Rollback transaction
                await transaction.RollbackAsync();

                // OPTIONAL: clean up files if needed
                // (safe guard if files were written before failure)

                return StatusCode(500, new
                {
                    message = "Failed to add product. Transaction rolled back.",
                    error = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPost("updateProduct")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto dto)
        {
            if (dto.ProductId <= 0)
            {

                return Ok(new ApiResponse<object>
                {
                    response_code = 0,
                    response_message = "fail"
                });
            }

            var product = await _context.TblProducts
                .FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);

            if (product == null)
            {

                return Ok(new ApiResponse<object>
                {
                    response_code = 0,
                    response_message = "fail"
                });
            }

            // Update name
            if (!string.IsNullOrWhiteSpace(dto.ProductName))
                product.ProductName = dto.ProductName;

            // Update description
            if (dto.ProductDescription != null)
                product.ProductDescription = dto.ProductDescription;

            // Update category (validate)
            if (dto.CategoryId.HasValue)
            {
                var categoryExists = await _context.TblProductcategories
                    .AnyAsync(c => c.CategoryId == dto.CategoryId && c.IsActive);

                if (!categoryExists)
                    return BadRequest("Invalid or inactive category.");

                product.CategoryId = dto.CategoryId.Value;
            }

            // Soft delete / restore
            if (dto.IsActive.HasValue)
                product.IsActive = dto.IsActive.Value;

            product.ModifiedOn = DateTime.UtcNow; // if column exists

            await _context.SaveChangesAsync();

            //return Ok(new
            //{
            //    message = "Product updated successfully",
            //    productId = product.ProductId
            //});
            return Ok(new ApiResponse<object>
            {
                response_code = 1,
                response_message = "success",
                data = new
                {
                    message = "Product updated successfully",
                    productId = product.ProductId

                }
            });
        }
        [AllowAnonymous]
        [HttpPost("getProductByID")]
        public async Task<IActionResult> GetProductByID([FromBody] JsonElement body)
        {
            if (!body.TryGetProperty("ProductID", out JsonElement idElement) ||
                idElement.ValueKind != JsonValueKind.Number)
            {
                return BadRequest("ProductID is required and must be a number.");
            }

            int productID = idElement.GetInt32();

            var product = await _context.TblProducts
                .Where(p => p.ProductId == productID)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.ProductDescription,
                    p.CategoryId,
                    CategoryName = p.Category.CategoryName,
                    p.IsActive,
                    p.CreatedOn,

                    PrimaryImage = p.TblProductImages
                        .Where(i => i.IsPrimary && i.IsActive)
                        .Select(i => i.ImagePath)
                        .FirstOrDefault(),

                    Images = p.TblProductImages
                        .Where(i => i.IsActive)
                        .Select(i => new
                        {
                            i.ImageId,
                            i.ImagePath,
                            i.IsPrimary
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound("Product not found.");

            return Ok(product);
        }

        [AllowAnonymous]
        [HttpPost("getAllProduct")]
        public async Task<IActionResult> GetAllProduct()
        {
            var products = await _context.TblProducts
                .Where(p => p.IsActive) // optional but recommended
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.ProductDescription,
                    p.CategoryId,
                    CategoryName = p.Category.CategoryName,
                    p.IsActive,
                    p.CreatedOn,

                    PrimaryImage = p.TblProductImages
                        .Where(i =>  i.IsActive)
                        .Select(i => i.ImagePath)
                        .FirstOrDefault(),

                    Images = p.TblProductImages
                        .Where(i => i.IsActive)
                        .Select(i => new
                        {
                            i.ImageId,
                            i.ImagePath,
                            i.IsPrimary
                        })
                        .ToList()
                })
                .OrderByDescending(p => p.CreatedOn)
                .ToListAsync();

            return Ok(new
            {
                response_code = 1,
                response_message = "Products fetched successfully",
                data = products
            });
        }



    }
}
