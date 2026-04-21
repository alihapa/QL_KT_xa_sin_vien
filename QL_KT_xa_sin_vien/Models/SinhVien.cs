using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QL_KT_xa_sin_vien.Models;

public partial class SinhVien
{
    [Required]
    public string MaSv { get; set; } = null!;

    [StringLength(200)]
    public string? HoTen { get; set; }

    [StringLength(50)]
    public string? Lop { get; set; }

    [StringLength(100)]
    public string? Khoa { get; set; }

    [StringLength(20)]
    public string? SoCmnd { get; set; }

    [StringLength(20)]
    public string? GioiTinh { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }
    public string? MaTaiKhoan { get; set; }

    public virtual ICollection<Giuong> Giuongs { get; set; } = new List<Giuong>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual ICollection<HopDong> HopDongs { get; set; } = new List<HopDong>();

    public virtual TaiKhoan? MaTaiKhoanNavigation { get; set; }

    public virtual ICollection<PhanAnh> PhanAnhs { get; set; } = new List<PhanAnh>();
}
