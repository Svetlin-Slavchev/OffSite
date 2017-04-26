using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using OffSite.Abstraction.Entities;
using OffSite.Abstraction.Interfaces;
using System.IO;

namespace OffSite.Data.Services
{
    public class AuthMessageSender : IEmailSender
    {
        public void SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Test", "todo@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(TextFormat.Html) { Text = message };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587);
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Use real gmail account.
                client.Authenticate("todo@gmail.com", "todo");

                client.Send(emailMessage);
                client.Disconnect(true);
            }
        }

        public string GetBody(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            using (var reader = new StreamReader(stream))
            {
                var text = reader.ReadToEnd();
                return text;
            }
        }

        public string EmailTemplate(string messageBody, ApplicationUser watcher, string requestLink)
        {
            messageBody = messageBody.Replace("{|username|}", watcher.UserName);
            messageBody = messageBody.Replace("{|requestViewLink|}", requestLink);

            return messageBody;
        }
    }
}
