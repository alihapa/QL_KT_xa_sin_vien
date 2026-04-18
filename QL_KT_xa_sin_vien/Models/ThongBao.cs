using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QL_KT_xa_sin_vien.Models;

public partial class ThongBao
{
    [Required]
    public string MaThongBao { get; set; } = null!;

    [Required]
    public string NguoiGui { get; set; } = null!;

    public string? NguoiNhan { get; set; }

    [StringLength(50)]
    public string? LoaiThongBao { get; set; }

    [StringLength(1000)]
    public string? NoiDung { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? ThoiGianGui { get; set; }

    public virtual TaiKhoan? NguoiNhanNavigation { get; set; }
}
