using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using OTP_Helper.Interfaces;
using OTP_Helper.Settings;

namespace OTP_Helper.Services
{
    public class MailKitEmailService : IEmailService
    {
        private readonly ILogger<MailKitEmailService> _logger;
        private readonly SmtpSettings _settings;

        public MailKitEmailService(IOptions<SmtpSettings> options, ILogger<MailKitEmailService> logger)
        {
            _logger = logger;
            _settings = options.Value ?? throw new ArgumentNullException(nameof(options));

            _logger.LogInformation(
                "MailKitEmailService initialized. Host={Host}, Port={Port}, From={From}",
                _settings.Host, _settings.Port, _settings.From);
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_settings.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();

            try
            {
                _logger.LogInformation("Attempting SMTP connection to {Host}:{Port}", _settings.Host, _settings.Port);

                await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, ct);
                _logger.LogInformation("SMTP connection established.");

                await client.AuthenticateAsync(_settings.Username, _settings.Password, ct);
                _logger.LogInformation("SMTP authentication successful.");

                await client.SendAsync(message, ct);
                _logger.LogInformation("Email successfully sent to {Email}", to);
            }
            catch (SmtpCommandException ex) // <-- from MailKit.Net.Smtp
            {
                _logger.LogError(ex, "SMTP command failed. StatusCode={StatusCode}, Response={Response}", ex.StatusCode, ex.Message);
                throw;
            }
            catch (SmtpProtocolException ex) // <-- from MailKit.Net.Smtp
            {
                _logger.LogError(ex, "SMTP protocol error: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}. Error: {Message}", to, ex.Message);
                throw;
            }
            finally
            {
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(true, ct);
                    _logger.LogInformation("SMTP client disconnected.");
                }
            }
        }
    }
}
