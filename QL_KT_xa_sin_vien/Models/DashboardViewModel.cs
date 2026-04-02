using Microsoft.AspNetCore.Mvc;

namespace QL_KT_xa_sin_vien.Models
{
    public class DashboardViewModel 
    {
        public SinhVien SinhVien { get; set; }
        public Phong Phong { get; set; }
        public HopDong HopDong { get; set; }
        public List<HoaDon> HoaDons { get; set; }
        public List<PhanAnh> PhanAnhs { get; set; }
        public List<ThongBao> ThongBaos { get; set; }
    }
}
