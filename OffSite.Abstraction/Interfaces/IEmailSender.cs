using System.Threading.Tasks;
using OffSite.Abstraction.Entities;

namespace OffSite.Abstraction.Interfaces
{
    public interface IEmailSender
    {
        //Task SendEmailAsync(string email, string subject, string message);
        void SendEmailAsync(string email, string subject, string message);

        string GetBody(string path);

        string EmailTemplate(string messageBody, ApplicationUser watcher, string requestLink);
    }
}
