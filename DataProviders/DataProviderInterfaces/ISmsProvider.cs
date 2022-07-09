using MyLibraries;
using System.Net;
using System.Threading.Tasks;

namespace DataProviderInterfaces
{
    public interface ISmsProvider
    {
        Task Send(NetworkCredential networkCredential, SMSMessage sms);
    }
}
