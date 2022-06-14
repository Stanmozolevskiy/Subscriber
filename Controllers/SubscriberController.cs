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

        [HttpGet("getDataFromFacebook/{subject}")]
        public async Task<IActionResult> GetDataFromFacebook([FromBody] string subject)=>
            Ok(await facebookProvider.GetDataFromFacebook(await facebookProvider.GetBrowserPage(), subject));

        [HttpGet("getDataListFromFirebase/{subject}")]
        public async Task<IActionResult> GetDataFromFirebase([FromBody] string subject)=>
            Ok(await firebaseProvider.GetDataFromFirebase(firebaseProvider.GetFirebaseClient(configuration), subject));
        
        [HttpPost("postDataFromFirebase")]
        public async Task<IActionResult> PostDataToFirebase(string subject, Data data)=>
            Ok(await firebaseProvider.SendDataToFirebase(firebaseProvider.GetFirebaseClient(configuration), subject, data));

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] string phone) =>
           Ok(await firebaseProvider.Register(firebaseProvider.GetFirebaseClient(configuration), phone));


        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] Credantials credantials)
        {
            string path = credantials.Phone + "/" + credantials.Subject;
            FireSharp.FirebaseClient firebaseClent = firebaseProvider.GetFirebaseClient(configuration);

            // Create a subscription 
            var isSubscribed =  await firebaseProvider.SendDataToFirebase(
                                                    firebaseClent,
                                                    path+"-subscribed",
                                                    true);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
                {
                    //check if still subscribed
                    CancellationTokenSource cancellationToken =  new CancellationTokenSource();

                    Dictionary<string, bool> result = JsonConvert.DeserializeObject<Dictionary<string, bool>>(
                                                   (await firebaseClent.GetAsync(path + "-subscribed")).Body);
                    while (result.Count < 2)
                    {
                       result = JsonConvert.DeserializeObject<Dictionary<string, bool>>(
                                                   (await firebaseClent.GetAsync(path + "-subscribed")).Body);
                        if (result is not null && result.Count >= 2)
                                cancellationToken.Cancel();

                        int randomTime = new Random().Next(1, 2);
                        await runFunction(firebaseClent, path, credantials);
                        await Task.Delay(randomTime * 100000, cancellationToken.Token);
                        if (cancellationToken.IsCancellationRequested)
                            break;
                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return Ok("Subscribed");
        }

        [HttpPost("undubscribe")]
        public async Task<IActionResult> Undubscribe([FromBody] Credantials credantials)
        {
            string path = credantials.Phone + "/" + credantials.Subject;
            await firebaseProvider.SendDataToFirebase(firebaseProvider.GetFirebaseClient(configuration),
                                                    path + "-subscribed",
                                                    false);
            firebaseProvider.GetFirebaseClient(configuration).Delete(path);
            firebaseProvider.GetFirebaseClient(configuration).Delete(path + "-subscribed");
            return Ok("Unsubscribed");
        }

        private async Task runFunction(FireSharp.FirebaseClient firebaseClent, string path, Credantials credantials) {

            Dictionary<string, Data> OldDataFromFirebase = await firebaseProvider
                                                                .GetDataFromFirebase(firebaseClent, path);

            List<Data> newDataFromFaceBook = await facebookProvider.GetDataFromFacebook(
                                         await facebookProvider.GetBrowserPage(), credantials.Subject);

            foreach (Data item in newDataFromFaceBook)
                await firebaseProvider.AddItemToFirebaseList(
                                path,
                                    OldDataFromFirebase is null ? new List<Data>() :
                                    OldDataFromFirebase.Select(oldItem => oldItem.Value).ToList(),
                                item,
                                configuration);

        }



        private IFirebaseProvider firebaseProvider;
        private IConfiguration configuration;
        private IFacebookProvider facebookProvider;
    }
}
