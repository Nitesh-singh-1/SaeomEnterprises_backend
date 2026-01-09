using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace saeom_backend.Models;

[Table("tbl_user")]
[Index("UserName", Name = "IX_tbl_user_UserName")]
[Index("UserName", Name = "UQ__tbl_user__C9F2845645A7C1C1", IsUnique = true)]
public partial class TblUser
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(100)]
    public string UserName { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(255)]
    public string PasswordSalt { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<TblProductImage> TblProductImages { get; set; } = new List<TblProductImage>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<TblProductcategory> TblProductcategories { get; set; } = new List<TblProductcategory>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<TblProduct> TblProducts { get; set; } = new List<TblProduct>();
}
