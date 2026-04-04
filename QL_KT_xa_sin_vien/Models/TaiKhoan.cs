using System;
using System.Collections.Generic;

namespace QL_KT_xa_sin_vien.Models;

public partial class TaiKhoan
{
    public string MaTaiKhoan { get; set; } = null!;

    public string TenDangNhap { get; set; } = null!;

    public string? MatKhauMh { get; set; }

    public string? Email { get; set; }

    public string? Sdt { get; set; }

    public string? VaiTro { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<NhatKy> NhatKies { get; set; } = new List<NhatKy>();

    public virtual ICollection<PhanAnh> PhanAnhs { get; set; } = new List<PhanAnh>();

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();

    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();

    public virtual VaiTro? VaiTroNavigation { get; set; }
}
