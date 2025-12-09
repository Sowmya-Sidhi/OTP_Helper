namespace OTP_Helper.Services.Interfaces
{
    public interface IPasswordResetService
    {
        Task RequestPasswordResetAsync(string email);
        Task VerifyOtpAsync(string email, string otp);
        Task ResetPasswordWithOtpAsync(string email, string otp, string newPassword);
    }
}