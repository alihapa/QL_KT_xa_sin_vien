using System;
using System.Collections.Generic;

namespace QL_KT_xa_sin_vien.Models;

public partial class ThongBao
{
    public string MaThongBao { get; set; } = null!;

    public string? NguoiNhan { get; set; }

    public string? LoaiThongBao { get; set; }

    public string? NoiDung { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? ThoiGianGui { get; set; }

    public virtual TaiKhoan? NguoiNhanNavigation { get; set; }
}
