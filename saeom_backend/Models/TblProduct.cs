using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace saeom_backend.Models;

[Table("tbl_product")]
[Index("CategoryId", Name = "IX_Product_CategoryID")]
[Index("IsActive", Name = "IX_Product_IsActive")]
public partial class TblProduct
{
    [Key]
    [Column("ProductID")]
    public int ProductId { get; set; }

    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [StringLength(200)]
    public string ProductName { get; set; } = null!;

    public string? ProductDescription { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("TblProducts")]
    public virtual TblProductcategory Category { get; set; } = null!;

    [ForeignKey("ModifiedBy")]
    [InverseProperty("TblProducts")]
    public virtual TblUser? ModifiedByNavigation { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<TblProductImage> TblProductImages { get; set; } = new List<TblProductImage>();
}
