using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QL_KT_xa_sin_vien.Models;

public partial class Phong
{
    [Required]
    public string MaPhong { get; set; } = null!;

    public string? MaToaNha { get; set; }

    public int? Tang { get; set; }

    [StringLength(50)]
    public string? LoaiPhong { get; set; }

    public int? SucChua { get; set; }

    public int? SoLuongDangO { get; set; }

    [StringLength(10)]
    public string? GioiTinh { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    public virtual ICollection<Giuong> Giuongs { get; set; } = new List<Giuong>();

    public virtual ICollection<HopDong> HopDongs { get; set; } = new List<HopDong>();

    public virtual ToaNha? MaToaNhaNavigation { get; set; }

    public virtual ICollection<PhanAnh> PhanAnhs { get; set; } = new List<PhanAnh>();
}
