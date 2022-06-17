using DataModels;
using DataProviderInterfaces;
using FireSharp;
using FireSharp.Config;
using FireSharp.Response;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FirebaseProvider
{
    public class Provider : IFirebaseProvider
    {
        public Provider(IFacebookProvider facebookProvider)
        {
            this.facebookProvider = facebookProvider;
        }
        public async Task AddItemToFirebaseList(string subject, List<Data> dataDictionary, Data data, IConfiguration configuration)
        {
            if(!dataDictionary.Any(x => x.Url == data.Url))
             await SendDataToFirebase(GetFirebaseClient(configuration), subject, data);
        }

        public async Task<Dictionary<string, Data>> GetDataFromFirebase(FirebaseClient firebaseClient, string subject)
        {
            Dictionary<string, Data> data = JsonConvert.DeserializeObject<Dictionary<string, Data>>(
                                            (await firebaseClient.GetAsync(subject)).Body);
            firebaseClient.Dispose();
            return data;
        }

        public FirebaseClient GetFirebaseClient(IConfiguration configuration) => new FirebaseClient(new FirebaseConfig
        {
            AuthSecret = configuration["Firebase:AuthSecret"],
            BasePath = configuration["Firebase:BasePath"]
        });

        public async Task<string> Register(FirebaseClient firebaseClient, string phone)
        {
            FirebaseResponse data = await firebaseClient.GetAsync(phone);
            if (data.Body is not "null")
            {
                firebaseClient.Dispose();
                return $"{phone} Already Regestered";
            }
           
            await firebaseClient.PushAsync(phone, phone);
            firebaseClient.Dispose();
            return $"{phone} Regestered";
        }

        public async Task<Dictionary<string, SubscriptionName>> GetSubscriptions(FirebaseClient firebaseClient, string phone)
        {
            if(phone.Length == 10)
            {
            FirebaseResponse resp = await firebaseClient.GetAsync($"{phone}/subscriptions");
            Dictionary<string, SubscriptionName> data = JsonConvert.DeserializeObject<Dictionary<string, SubscriptionName>>(resp.Body);
            firebaseClient.Dispose();
                return data;
            }
            return null;
        }

        public async Task<PushResponse> SendDataToFirebase<T>(FirebaseClient firebaseClient, string subject, T data)
        {
            PushResponse response = await firebaseClient.PushAsync(subject, data);
            firebaseClient.Dispose();
            return response;
        }

        public async Task<string> Unsubscribe(Credantials credantials, IConfiguration configuration)
        {
            FireSharp.FirebaseClient firebaseClent = GetFirebaseClient(configuration);
            KeyValuePair<string, SubscriptionName> itemToUnsubscribe = (await isSubscribed(firebaseClent, credantials));
            // Remove a record to a subscription folder in the db 
            await firebaseClent.DeleteAsync($"{credantials.Phone}/subscriptions/{itemToUnsubscribe.Key}");
            // Remove db records
            await firebaseClent.DeleteAsync($"{credantials.Phone}/{itemToUnsubscribe.Value?.Name}");
            firebaseClent.Dispose();
            return "Unsubscribed";
        }

        public async Task<string> Subscribe(Credantials credantials, IConfiguration configuration)
        {
            string path = $"{credantials.Phone}/{credantials.Subject}";
            FireSharp.FirebaseClient firebaseClent = GetFirebaseClient(configuration);

            // Add a record to a subscription folder in the db 
            await SendDataToFirebase(firebaseClent, $"{credantials.Phone}/subscriptions", new SubscriptionName(credantials.Subject));


#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                //check if still subscribed
                CancellationTokenSource cancellationToken = new CancellationTokenSource();

                while ((await isSubscribed(firebaseClent, credantials)).Value is not null)
                {
                    if ((await isSubscribed(firebaseClent, credantials)).Value is not null)
                        cancellationToken.Cancel();

                    int randomTime = new Random().Next(1, 2);
                    await populateFacebookData(firebaseClent, path, credantials, configuration);
                    await Task.Delay(randomTime * 100000, cancellationToken.Token);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        firebaseClent.Dispose();
                        break;
                    }
                }
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            return "Subscribed";
        }


        private async Task populateFacebookData(FirebaseClient firebaseClent, string path, Credantials credantials, IConfiguration configuration)
        {

            Dictionary<string, Data> OldDataFromFirebase = await GetDataFromFirebase(firebaseClent, path);

            List<Data> newDataFromFaceBook = await facebookProvider.GetDataFromFacebook(
                                             await facebookProvider.GetBrowserPage(), credantials.Subject);

            foreach (Data item in newDataFromFaceBook)
                await AddItemToFirebaseList(path,
                                    OldDataFromFirebase is null ? new List<Data>() :
                                    OldDataFromFirebase.Select(oldItem => oldItem.Value).ToList(),
                                    item,
                                    configuration);

        }

        private async Task<KeyValuePair<string, SubscriptionName>> isSubscribed(FirebaseClient firebaseClent, Credantials credantials) =>
               (await GetSubscriptions(firebaseClent, credantials.Phone)).FirstOrDefault(x => x.Value?.Name == credantials.Subject);


        private IFacebookProvider facebookProvider;

    }
}
