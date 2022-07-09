using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net;
using MyLibraries;
using DataProviderInterfaces;

namespace Subscriber.Controllers
{
    [Route("[controller]"), ApiController, AllowAnonymous]
    public class SMSController : ControllerBase
    {
        public SMSController(IConfiguration configuration, ISmsProvider smsProvider )
        {
            this.smsProvider = smsProvider;
            networkCredential = new NetworkCredential(configuration["Settings:Tvilio:accountSid"],
                configuration["Settings:Tvilio:authToken"], configuration["Settings:Tvilio:domain"]);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendSMS(SMSMessage sms)
        {
            await smsProvider.Send(networkCredential, sms);
            return Ok();
        }


        private readonly NetworkCredential networkCredential;
        private readonly ISmsProvider smsProvider;
    
    }
}
