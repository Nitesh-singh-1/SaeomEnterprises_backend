using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace saeom_backend.Models;

[Table("tbl_product_images")]
[Index("ProductId", Name = "IX_ProductImages_ProductID")]
public partial class TblProductImage
{
    [Key]
    [Column("ImageID")]
    public int ImageId { get; set; }

    [Column("ProductID")]
    public int ProductId { get; set; }

    [StringLength(500)]
    public string ImagePath { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("TblProductImages")]
    public virtual TblUser? ModifiedByNavigation { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("TblProductImages")]
    public virtual TblProduct Product { get; set; } = null!;
}
