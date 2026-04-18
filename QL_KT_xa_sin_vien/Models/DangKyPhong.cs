using System.ComponentModel.DataAnnotations;

namespace QL_KT_xa_sin_vien.Models
{
    public class DangKyPhong
    {
        [Required]
        public string MaSv { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string HoTen { get; set; }

        // Giá trị được chọn
        public string? SelectedToaNha { get; set; }
        public string? SelectedPhong { get; set; }
        public string? SelectedGiuong { get; set; }

        // Danh sách để hiển thị trong select
        public List<string> ToaNhaList { get; set; } = new();
        public List<string> PhongList { get; set; } = new();
        public List<string> GiuongList { get; set; } = new();
    }

}
