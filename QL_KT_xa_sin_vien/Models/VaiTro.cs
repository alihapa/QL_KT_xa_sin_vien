using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QL_KT_xa_sin_vien.Models;

public partial class VaiTro
{
    [Required]
    public string MaVaiTro { get; set; } = null!;

    [StringLength(100)]
    public string? TenVaiTro { get; set; }

    public string? QuyenHan { get; set; }

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
