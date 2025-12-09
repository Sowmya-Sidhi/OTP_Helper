using OTP_Helper.Models;

namespace OTP_Helper.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);

        Task CreatePasswordResetTokenAsync(PasswordResetToken token);
        Task<PasswordResetToken?> GetValidPasswordResetTokenAsync(string userId);

        Task IncrementPasswordResetAttemptsAsync(string tokenId);
        Task MarkPasswordResetTokenUsedAsync(string tokenId);

        Task UpdateUserPasswordAsync(string userId, string newPasswordHash);
    }
}
