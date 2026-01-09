using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace saeom_backend.Models;

[Table("tbl_contactus")]
[Index("Email", Name = "IX_ContactUs_Email")]
public partial class TblContactu
{
    [Key]
    [Column("ContactID")]
    public int ContactId { get; set; }

    [StringLength(150)]
    public string FullName { get; set; } = null!;

    [StringLength(20)]
    public string ContactNumber { get; set; } = null!;

    [StringLength(150)]
    public string Email { get; set; } = null!;

    [StringLength(150)]
    public string? CompanyName { get; set; }

    [Column("ProductCategoryID")]
    public int? ProductCategoryId { get; set; }

    public string QueryMade { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    [ForeignKey("ProductCategoryId")]
    [InverseProperty("TblContactus")]
    public virtual TblProductcategory? ProductCategory { get; set; }
}
