using DataProviderInterfaces;
using MyLibraries;
using System.Net;
using System.Threading.Tasks;

namespace TwilioProvider
{
    public class Provider: ISmsProvider
    {
        public async Task Send(NetworkCredential networkCredential, SMSMessage sms)
        {
            await SMS.Send(networkCredential, sms);
        }
    }
}
