using System;
using System.ComponentModel.DataAnnotations;

namespace QL_KT_xa_sin_vien.Models;

public partial class DonGia
{
    [Key]
    public string MaDonGia { get; set; } = Guid.NewGuid().ToString();

    // price per kWh
    public decimal? DonGiaDien { get; set; }

    // price per m3
    public decimal? DonGiaNuoc { get; set; }

    // price per month for room
    public decimal? DonGiaPhong { get; set; }

    // default usages applied when generating invoices (can be adjusted)
    public decimal? DienUsageDefault { get; set; }
    public decimal? NuocUsageDefault { get; set; }

    public DateTime? NgayHieuLuc { get; set; }
}
