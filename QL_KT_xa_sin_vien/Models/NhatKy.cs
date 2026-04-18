using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QL_KT_xa_sin_vien.Models;

public partial class NhatKy
{
    [Required]
    public string MaLog { get; set; } = null!;

    public string? NguoiThucHien { get; set; }

    [StringLength(100)]
    public string? HanhDong { get; set; }

    [StringLength(200)]
    public string? DoiTuong { get; set; }

    public string? GiaTriTruoc { get; set; }

    public string? GiaTriSau { get; set; }

    public DateTime? ThoiGian { get; set; }

    public virtual TaiKhoan? NguoiThucHienNavigation { get; set; }
}
