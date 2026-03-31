using System;
using System.Collections.Generic;

namespace QL_KT_xa_sin_vien.Models;

public partial class ToaNha
{
    public string MaToaNha { get; set; } = null!;

    public string? TenToaNha { get; set; }

    public string? DiaChi { get; set; }

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
