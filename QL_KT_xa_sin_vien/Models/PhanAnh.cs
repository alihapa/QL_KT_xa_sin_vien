using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QL_KT_xa_sin_vien.Models;

    using System.ComponentModel.DataAnnotations.Schema;

    public partial class PhanAnh
    {
    [Required]
    public string MaPhanAnh { get; set; } = null!;

    public string? MaSv { get; set; }

    // Tài khoản người gửi (MaTaiKhoan)
    [NotMapped]
    public string? NguoiGoi { get; set; }

    // Nếu true thì ẩn danh với các user role = 1
    [NotMapped]
    public bool? AnDanh { get; set; }

    public string? MaPhong { get; set; }

    [StringLength(1000)]
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
