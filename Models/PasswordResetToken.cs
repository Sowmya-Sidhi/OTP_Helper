namespace OTP_Helper.Models
{
    public class PasswordResetToken
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string TokenHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public bool Used { get; set; }
        public int Attempts { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
