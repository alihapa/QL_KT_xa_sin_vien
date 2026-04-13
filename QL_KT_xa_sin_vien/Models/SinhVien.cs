using System;
using System.Collections.Generic;

namespace QL_KT_xa_sin_vien.Models;

public partial class SinhVien
{
    public string MaSv { get; set; } = null!;

    public string? HoTen { get; set; }

    public string? Lop { get; set; }

    public string? Khoa { get; set; }

    public string? SoCmnd { get; set; }

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
