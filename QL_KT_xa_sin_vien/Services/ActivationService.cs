using System.Collections.Concurrent;

namespace QL_KT_xa_sin_vien.Services
{
    public class ActivationInfo
    {
        public string Code { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
        public DateTime LastSent { get; set; }
        public int SentCountToday { get; set; }
        public DateTime CountDate { get; set; }
    }

    public static class ActivationService
    {
        private static readonly ConcurrentDictionary<string, ActivationInfo> _store = new(StringComparer.OrdinalIgnoreCase);
        private static readonly object _lock = new();

        // Generates a 6-digit numeric code
        private static string GenerateCode()
        {
            var bytes = new byte[4];
            System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
            var value = BitConverter.ToUInt32(bytes, 0) % 1000000;
            return value.ToString("D6");
        }

        // Try send code, enforces cooldown (30s) and daily limit (10)
        public static (bool Success, string Message, int SecondsUntilResend, string Code) TrySendCode(string email, TimeSpan codeExpiry, int dailyLimit = 10, int resendCooldownSeconds = 30, string? username = null)
        {
            var now = DateTime.UtcNow;
            var key = string.IsNullOrEmpty(username) ? email : $"{email}|{username}";
            var info = _store.GetOrAdd(key, _ => new ActivationInfo { Code = string.Empty, Expiry = DateTime.MinValue, LastSent = DateTime.MinValue, SentCountToday = 0, CountDate = now.Date });

            lock (_lock)
            {
                // reset daily count if date changed
                if (info.CountDate.Date != now.Date)
                {
                    info.SentCountToday = 0;
                    info.CountDate = now.Date;
                }

                // check daily limit
                if (info.SentCountToday >= dailyLimit)
                {
                    return (false, "Đã đạt giới hạn gửi lại cho hôm nay.", 0, string.Empty);
                }

                // check cooldown
                var secondsSinceLast = (now - info.LastSent).TotalSeconds;
                if (secondsSinceLast < resendCooldownSeconds)
                {
                    int wait = (int)Math.Ceiling(resendCooldownSeconds - secondsSinceLast);
                    return (false, "Vui lòng chờ trước khi gửi lại.", wait, string.Empty);
                }

                // generate and store
                info.Code = GenerateCode();
                info.Expiry = now.Add(codeExpiry);
                info.LastSent = now;
                info.SentCountToday++;

                _store[key] = info;
                return (true, "Mã kích hoạt đã được gửi.", 0, info.Code);
            }
        }

        public static (bool Success, string Message) VerifyCode(string email, string code, string? username = null)
        {
            var key = string.IsNullOrEmpty(username) ? email : $"{email}|{username}";
            if (!_store.TryGetValue(key, out var info)) return (false, "Không tìm thấy mã kích hoạt. Vui lòng yêu cầu gửi lại.");
            if (DateTime.UtcNow > info.Expiry) return (false, "Mã đã hết hạn. Vui lòng yêu cầu mã mới.");
            if (!string.Equals(info.Code, code, StringComparison.OrdinalIgnoreCase)) return (false, "Mã không đúng.");

            // remove on success
            _store.TryRemove(key, out _);
            return (true, "Xác thực thành công.");
        }

        public static int SecondsUntilResend(string email, int resendCooldownSeconds = 30, string? username = null)
        {
            var key = string.IsNullOrEmpty(username) ? email : $"{email}|{username}";
            if (!_store.TryGetValue(key, out var info)) return 0;
            var since = (DateTime.UtcNow - info.LastSent).TotalSeconds;
            if (since >= resendCooldownSeconds) return 0;
            return (int)Math.Ceiling(resendCooldownSeconds - since);
        }
    }
}
