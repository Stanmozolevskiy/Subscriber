using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using DataProviderInterfaces;
using DataModels;
using System.Linq;

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
        public async Task<IActionResult> PostDataFromFirebase(string subject, Data data)=>
            Ok(await firebaseProvider.SendDataToFirebase(firebaseProvider.GetFirebaseClient(configuration), 
                subject, data));


        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] string subject)
        {
            Dictionary<string, Data> OldDataFromFirebase = await firebaseProvider
                                                            .GetDataFromFirebase(firebaseProvider
                                                            .GetFirebaseClient(configuration), subject);

            List<Data> newDataFromFaceBook = await facebookProvider.GetDataFromFacebook(await facebookProvider.GetBrowserPage(), subject);

            foreach(Data item in newDataFromFaceBook)
                await firebaseProvider.AddItemToFirebaseList(
                    subject,
                    OldDataFromFirebase is null ? new List<Data>() : OldDataFromFirebase.Select(oldItem => oldItem.Value).ToList(),
                    item,
                    configuration);

            return Ok(newDataFromFaceBook.Count());
        }


        private IFirebaseProvider firebaseProvider;
        private IConfiguration configuration;
        private IFacebookProvider facebookProvider;
    }
}
