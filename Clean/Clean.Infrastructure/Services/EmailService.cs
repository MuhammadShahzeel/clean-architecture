using Clean.Application.DTOs;
using Clean.Application.Interfaces;
using Clean.Application.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Clean.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

     
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendAsync(EmailRequest request)
        {
            try
            {
                var message = new MimeMessage();
                message.Sender = new MailboxAddress(_emailSettings.DisplayName, _emailSettings.EmailFrom);
                message.To.Add(MailboxAddress.Parse(request.To));
                message.Subject = request.Subject;

                var builder = new BodyBuilder();
                if (request.IsHtmlBody)
                    builder.HtmlBody = request.Body;
                else
                    builder.TextBody = request.Body;

                message.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            { }
        }
    }
}