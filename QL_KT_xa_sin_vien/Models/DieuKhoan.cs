using System;
using System.ComponentModel.DataAnnotations;

namespace QL_KT_xa_sin_vien.Models;

public partial class DieuKhoan
{
    [Required]
    public string MaDieuKhoan { get; set; } = null!;

    // Relative path under wwwroot, e.g. "/uploads/xxx.pdf"
    [StringLength(500)]
    public string? FilePath { get; set; }

    [StringLength(500)]
    public string? OriginalFileName { get; set; }

    [StringLength(50)]
    public string? UploadedBy { get; set; }

    public DateTime? UploadedAt { get; set; }
}
