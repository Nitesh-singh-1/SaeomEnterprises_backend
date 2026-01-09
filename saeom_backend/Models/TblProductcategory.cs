using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace saeom_backend.Models;

[Table("tbl_productcategory")]
[Index("IsActive", Name = "IX_ProductCategory_IsActive")]
public partial class TblProductcategory
{
    [Key]
    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [StringLength(150)]
    public string CategoryName { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("TblProductcategories")]
    public virtual TblUser? ModifiedByNavigation { get; set; }

    [InverseProperty("ProductCategory")]
    public virtual ICollection<TblContactu> TblContactus { get; set; } = new List<TblContactu>();

    [InverseProperty("Category")]
    public virtual ICollection<TblProduct> TblProducts { get; set; } = new List<TblProduct>();
}
