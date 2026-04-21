using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QL_KT_xa_sin_vien.Models;

public partial class HopDong
{
    [Required]
    public string MaHopDong { get; set; } = null!;

    public string? MaSv { get; set; }

    public string? MaPhong { get; set; }

    public string? MaGiuong { get; set; }

    public DateOnly? NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    public string? TrangThai { get; set; }

    public string? DieuKhoan { get; set; }
    // If contract includes a PDF link or acceptance flag
    [StringLength(500)]
    public string? DieuKhoanPdf { get; set; }

    // Agreement: "1" agree, "2" disagree
    [StringLength(10)]
    public string? Agree { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual Giuong? MaGiuongNavigation { get; set; }

    public virtual Phong? MaPhongNavigation { get; set; }

    public virtual SinhVien? MaSvNavigation { get; set; }
}
