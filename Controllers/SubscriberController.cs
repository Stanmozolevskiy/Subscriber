using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using DataProviderInterfaces;
using DataModels;
using System.Linq;

namespace Subscriber.Controllers
{
    [Route("[controller]"), ApiController, AllowAnonymous]
    public class SubscriberController: ControllerBase
    {
        public SubscriberController(IConfiguration configuration, IFacebookProvider facebookProvider, IFirebaseProvider firebaseProvider)
        {
            this.firebaseProvider = firebaseProvider;
            this.configuration = configuration;
            this.facebookProvider = facebookProvider;
        }

        [HttpPost("getDataFromFacebook")]
        public async Task<IActionResult> GetDataFromFacebook([FromBody] string subject)=>
            Ok(await facebookProvider.GetDataFromFacebook(await facebookProvider.GetBrowserPage(), subject));

        [HttpPost("getDataListFromFirebase")]
        public async Task<IActionResult> GetDataFromFirebase([FromBody] string subject)
        {
            Dictionary<string, Data> response = await firebaseProvider
                                                      .GetDataFromFirebase(firebaseProvider
                                                      .GetFirebaseClient(), subject);
            if (response?.Count > 0)
                return Ok(response?.Values.ToList<Data>());

            return Ok("Please wait one minute for data to populate");
        }
        
        [HttpPost("postDataFromFirebase")]
        public async Task<IActionResult> PostDataToFirebase(string subject, Data data)=>
            Ok(await firebaseProvider.SendDataToFirebase(firebaseProvider.GetFirebaseClient(), subject, data));

        [HttpPost("getSubscription")]
        public async Task<IActionResult> GetSubscriptions([FromBody] string phone)
        {
            Dictionary<string, SubscriptionName> response = (await firebaseProvider
                                                                .GetSubscriptions(firebaseProvider
                                                                .GetFirebaseClient(), phone));
            if (response?.Count > 0)
               return Ok(response.Values?.ToList<SubscriptionName>());

           return Ok("Please wait one minute for data to populate");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] string phone) =>
          Ok(await firebaseProvider.Register(firebaseProvider.GetFirebaseClient(), phone));

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] Credantials credantials)
        {
            var data = await firebaseProvider.Subscribe(credantials);
            await firebaseProvider.UploadAttachmentsToFirebase();
            return Ok(data);
        }
        
        [HttpPost("undubscribe")]
        public async Task<IActionResult> Undubscribe([FromBody] Credantials credantials) =>
            Ok(await firebaseProvider.Unsubscribe(credantials));
        


        private IFirebaseProvider firebaseProvider;
        private IConfiguration configuration;
        private IFacebookProvider facebookProvider;
    }
}
