using Microsoft.Extensions.Logging;
using OTP_Helper.Helper;
using OTP_Helper.Helpers;
using OTP_Helper.Interfaces;
using OTP_Helper.Models;
using OTP_Helper.Services.Interfaces;

namespace OTP_Helper.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IUserRepository _repo;
        private readonly IEmailService _email;
        private readonly ILogger<PasswordResetService> _logger;

        private readonly int _otpDigits = 6;
        private readonly int _otpTtlMinutes = 15;
        private readonly int _maxAttempts = 5;

        public PasswordResetService(
            IUserRepository repo,
            IEmailService email,
            ILogger<PasswordResetService> logger)
        {
            _repo = repo;
            _email = email;
            _logger = logger;
        }

        public async Task RequestPasswordResetAsync(string email)
        {
            _logger.LogInformation("RequestPasswordResetAsync called for {Email}", email);

            var user = await _repo.GetUserByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("No user found with email {Email}", email);
                return;
            }

            var otp = OtpHelper.GenerateNumericOtp(_otpDigits);
            _logger.LogInformation("Generated OTP {Otp} for user {Email}", otp, email);

            var (hash, salt) = OtpHelper.HashOtp(otp);

            var token = new PasswordResetToken
            {
                UserId = user.Id!,
                TokenHash = hash,
                Salt = salt,
                Attempts = 0,
                Used = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_otpTtlMinutes)
            };

            await _repo.CreatePasswordResetTokenAsync(token);
            _logger.LogInformation("Password reset token created for user {Email}", email);

            var body = $@"Your password reset code is <b>{otp}</b>. Valid for {_otpTtlMinutes} minutes.";

            _logger.LogInformation("Sending OTP email to {Email}", email);
            await _email.SendEmailAsync(email, "Password Reset Code", body);
            _logger.LogInformation("OTP email sent to {Email}", email);
        }

        public async Task VerifyOtpAsync(string email, string otp)
        {
            _logger.LogInformation("VerifyOtpAsync called for {Email}", email);

            var user = await _repo.GetUserByEmailAsync(email)
                ?? throw new Exception("Invalid email");

            var token = await _repo.GetValidPasswordResetTokenAsync(user.Id!)
                ?? throw new Exception("Invalid or expired OTP");

            if (!OtpHelper.VerifyHashedOtp(otp, token.Salt, token.TokenHash))
            {
                await _repo.IncrementPasswordResetAttemptsAsync(token.Id);
                _logger.LogWarning("Incorrect OTP for user {Email}", email);
                throw new Exception("Incorrect OTP");
            }

            _logger.LogInformation("OTP verified successfully for {Email}", email);
        }

        public async Task ResetPasswordWithOtpAsync(string email, string otp, string newPassword)
        {
            _logger.LogInformation("ResetPasswordWithOtpAsync called for {Email}", email);

            var user = await _repo.GetUserByEmailAsync(email)
                ?? throw new Exception("Invalid email");

            var token = await _repo.GetValidPasswordResetTokenAsync(user.Id!)
                ?? throw new Exception("Invalid or expired OTP");

            if (!OtpHelper.VerifyHashedOtp(otp, token.Salt, token.TokenHash))
            {
                _logger.LogWarning("Incorrect OTP for password reset for user {Email}", email);
                throw new Exception("Incorrect OTP");
            }

            var hashed = await PasswordHelper.HashPasswordAsync(newPassword);

            await _repo.UpdateUserPasswordAsync(user.Id!, hashed);
            await _repo.MarkPasswordResetTokenUsedAsync(token.Id);

            _logger.LogInformation("Password successfully reset for user {Email}", email);
        }
    }
}
