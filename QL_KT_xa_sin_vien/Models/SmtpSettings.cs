namespace QL_KT_xa_sin_vien.Models
{
    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string User { get; set; }
        public string Password { get; set; } // sẽ được inject từ secrets
    }

}
