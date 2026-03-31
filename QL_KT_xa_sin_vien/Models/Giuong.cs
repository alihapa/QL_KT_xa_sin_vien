using System;
using System.Collections.Generic;

namespace QL_KT_xa_sin_vien.Models;

public partial class Giuong
{
    public string MaGiuong { get; set; } = null!;

    public string? MaPhong { get; set; }

    public string? SoGiuong { get; set; }

    public string? OccupiedBy { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<HopDong> HopDongs { get; set; } = new List<HopDong>();

    public virtual Phong? MaPhongNavigation { get; set; }

    public virtual SinhVien? OccupiedByNavigation { get; set; }
}
