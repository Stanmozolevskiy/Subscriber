using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Twilio;
using System.Net;
using MyLibraries;


namespace Sbuscriber.Controllers
{
    [Route("[controller]"), ApiController, AllowAnonymous]
    public class SMSController : ControllerBase
    {
        public SMSController(IConfiguration configuration)
        {
            this.configuration = configuration;
            accountSid = configuration["Settings:Tvilio:accountSid"];
            authToken = configuration["Settings:Tvilio:authToken"];
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendSMS(SMSMessage sms)
        {
            TwilioClient.Init(accountSid, authToken);
            await SMS.Send(new NetworkCredential(accountSid, authToken, configuration["Settings:Tvilio:domain"]), sms);
            return Ok();
        }


        private readonly string accountSid;
        private readonly string authToken;
        private readonly IConfiguration configuration;
    }
}
