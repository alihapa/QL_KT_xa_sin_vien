namespace QL_KT_xa_sin_vien.Models
{
    public class DashboardADMINViewModel
    {
        public int SoLuongSinhVien { get; set; }
        public int SoLuongPhong { get; set; }
        public int SoLuongHoaDon { get; set; }
        public int SoLuongPhanAnh { get; set; }

        public List<HopDong> HopDongs { get; set; }
        public List<PhanAnh> PhanAnhs { get; set; }
    }
}
