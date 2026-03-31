using System;
using System.Collections.Generic;

namespace QL_KT_xa_sin_vien.Models;

public partial class PhanAnh
{
    public string MaPhanAnh { get; set; } = null!;

    public string? MaSv { get; set; }

    public string? MaPhong { get; set; }

    public string? MoTa { get; set; }

    public string? MucDoUuTien { get; set; }

    public string? TrangThai { get; set; }

    public string? NguoiXuLy { get; set; }

    public DateTime? ThoiGianTao { get; set; }

    public DateTime? ThoiGianCapNhat { get; set; }

    public virtual Phong? MaPhongNavigation { get; set; }

    public virtual SinhVien? MaSvNavigation { get; set; }

    public virtual TaiKhoan? NguoiXuLyNavigation { get; set; }
}
