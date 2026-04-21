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

        [StringLength(500)]
        public string? LyDo { get; set; }

        // Agreement to terms: "1" = agree, "2" = disagree
        public string? Agree { get; set; }

        // Uploaded PDF file for terms (bound in the form)
        [System.Text.Json.Serialization.JsonIgnore]
        public Microsoft.AspNetCore.Http.IFormFile? DieuKhoanPdfFile { get; set; }

        // Optional start/end dates for the contract
        public DateOnly? NgayBatDau { get; set; }
        public DateOnly? NgayKetThuc { get; set; }

        // Danh sách để hiển thị trong select
        public List<string> ToaNhaList { get; set; } = new();
        public List<string> PhongList { get; set; } = new();
        public List<string> GiuongList { get; set; } = new();
    }

}
