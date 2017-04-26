using System.Threading.Tasks;

namespace OffSite.Abstraction.Interfaces
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
