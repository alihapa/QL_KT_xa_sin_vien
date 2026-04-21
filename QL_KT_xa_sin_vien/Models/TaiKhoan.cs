using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QL_KT_xa_sin_vien.Models;

public partial class TaiKhoan
{
    [Required]
    public string MaTaiKhoan { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string TenDangNhap { get; set; } = null!;

    public string? MatKhauMh { get; set; }

    [EmailAddress]
    [StringLength(256)]
    public string? Email { get; set; }

    [Phone]
    public string? Sdt { get; set; }

    public string? VaiTro { get; set; }

    // account status: "1" = active, "0" = pending activation, "-1" = banned
    public string? TrangThai { get; set; } = "0";

    public virtual ICollection<NhatKy> NhatKies { get; set; } = new List<NhatKy>();

    public virtual ICollection<PhanAnh> PhanAnhs { get; set; } = new List<PhanAnh>();

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();

    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();

    public virtual VaiTro? VaiTroNavigation { get; set; }

    // account expiration date (nullable). When reached, account will be cleaned up automatically.
    public DateTime? ThoiHan { get; set; }
}
