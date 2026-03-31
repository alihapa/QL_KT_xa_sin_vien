using System;
using System.Collections.Generic;

namespace QL_KT_xa_sin_vien.Models;

public partial class VaiTro
{
    public string MaVaiTro { get; set; } = null!;

    public string? TenVaiTro { get; set; }

    public string? QuyenHan { get; set; }

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
