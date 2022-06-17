using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using DataProviderInterfaces;
using DataModels;
using System.Linq;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace Sbuscriber.Controllers
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
        public async Task<IActionResult> GetDataFromFirebase([FromBody] string subject)=>
            Ok(await firebaseProvider.GetDataFromFirebase(firebaseProvider.GetFirebaseClient(configuration), subject));
        
        [HttpPost("postDataFromFirebase")]
        public async Task<IActionResult> PostDataToFirebase(string subject, Data data)=>
            Ok(await firebaseProvider.SendDataToFirebase(firebaseProvider.GetFirebaseClient(configuration), subject, data));

        [HttpPost("getSubscription")]
        public async Task<IActionResult> GetSubscriptions([FromBody] string phone) =>
            Ok(await firebaseProvider.GetSubscriptions(firebaseProvider.GetFirebaseClient(configuration), phone));

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] string phone) =>
          Ok(await firebaseProvider.Register(firebaseProvider.GetFirebaseClient(configuration), phone));

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] Credantials credantials) =>
            Ok(await firebaseProvider.Subscribe(credantials, configuration));
        
        [HttpPost("undubscribe")]
        public async Task<IActionResult> Undubscribe([FromBody] Credantials credantials) =>
            Ok(await firebaseProvider.Unsubscribe(credantials, configuration));
        


        private IFirebaseProvider firebaseProvider;
        private IConfiguration configuration;
        private IFacebookProvider facebookProvider;
    }
}
