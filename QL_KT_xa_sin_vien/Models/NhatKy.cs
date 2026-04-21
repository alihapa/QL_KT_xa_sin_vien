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

    // The DB currently restricts these columns to 100 chars. Enforce truncation at runtime
    // to avoid SqlException when saving long serialized payloads. Consider creating a
    // migration to expand the column sizes if you want to store full JSON.
    private string? _giaTriTruoc;
    private string? _giaTriSau;

    [StringLength(100)]
    public string? GiaTriTruoc
    {
        get => _giaTriTruoc;
        set
        {
            if (value == null) _giaTriTruoc = null;
            else _giaTriTruoc = value.Length > 100 ? value.Substring(0, 100) : value;
        }
    }

    [StringLength(100)]
    public string? GiaTriSau
    {
        get => _giaTriSau;
        set
        {
            if (value == null) _giaTriSau = null;
            else _giaTriSau = value.Length > 100 ? value.Substring(0, 100) : value;
        }
    }

    public DateTime? ThoiGian { get; set; }

    public virtual TaiKhoan? NguoiThucHienNavigation { get; set; }
}
