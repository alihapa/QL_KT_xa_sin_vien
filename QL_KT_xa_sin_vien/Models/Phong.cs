using System;
using System.Collections.Generic;

namespace QL_KT_xa_sin_vien.Models;

public partial class Phong
{
    public string MaPhong { get; set; } = null!;

    public string? ToaNha { get; set; }

    public int? Tang { get; set; }

    public string? LoaiPhong { get; set; }

    public int? SucChua { get; set; }

    public int? SoLuongDangO { get; set; }

    public string? GioiTinh { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<Giuong> Giuongs { get; set; } = new List<Giuong>();

    public virtual ICollection<HopDong> HopDongs { get; set; } = new List<HopDong>();

    public virtual ICollection<PhanAnh> PhanAnhs { get; set; } = new List<PhanAnh>();

    public virtual ToaNha? ToaNhaNavigation { get; set; }
}
