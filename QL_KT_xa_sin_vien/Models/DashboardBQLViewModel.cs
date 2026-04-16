namespace QL_KT_xa_sin_vien.Models
{
    public class DashboardBQLViewModel
    {
        public List<SinhVien> Sinh_Vien { get; set; }
        public int SoLuongSinhVien { get; set; }
        public List<PhanAnh> Phan_Anh { get; set; }
        public int SoLuongPhanAnh { get; set; }
        public List<HopDong> Hop_Dong { get; set; }
        public int SoLuongHopDong { get; set; }
        public List<HoaDon> Hoa_Don { get; set; }
        public int SoLuongHoaDon { get; set; }
        public List<ThongBao> Thong_Bao { get; set; }
        public int SoLuongThongBao { get; set; }
    }
}
