using System;
using System.Collections.Generic;

namespace QL_KT_xa_sin_vien.Models;

public partial class NhatKy
{
    public string MaLog { get; set; } = null!;

    public string? NguoiThucHien { get; set; }

    public string? HanhDong { get; set; }

    public string? DoiTuong { get; set; }

    public string? GiaTriTruoc { get; set; }

    public string? GiaTriSau { get; set; }

    public DateTime? ThoiGian { get; set; }

    public virtual TaiKhoan? NguoiThucHienNavigation { get; set; }
}
