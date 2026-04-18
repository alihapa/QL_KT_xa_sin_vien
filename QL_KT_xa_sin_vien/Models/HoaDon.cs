using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QL_KT_xa_sin_vien.Models;

public partial class HoaDon
{
    [Required]
    public string MaHoaDon { get; set; } = null!;

    public string? MaHopDong { get; set; }

    public string? MaSv { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? SoTien { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? NgayXuat { get; set; }

    public virtual HopDong? MaHopDongNavigation { get; set; }

    public virtual SinhVien? MaSvNavigation { get; set; }
}
