using System.Collections.Generic;

namespace QL_KT_xa_sin_vien.Models
{
    public class KichHoatRequestModel
    {
        public string? Email { get; set; }
        public string? SelectedUsername { get; set; }
        public List<string>? Usernames { get; set; }
    }
}
