using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QL_KT_xa_sin_vien.Models;

public partial class ToaNha
{
    [Required]
    public string MaToaNha { get; set; } = null!;

    [StringLength(200)]
    public string? TenToaNha { get; set; }

    [StringLength(500)]
    public string? DiaChi { get; set; }

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
