using DataModels;
using DataProviderInterfaces;
using Firebase.Auth;
using Firebase.Storage;
using FireSharp;
using FireSharp.Response;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FirebaseProvider
{
    public class Provider : IFirebaseProvider
    {
        public Provider(IFacebookProvider facebookProvider, IConfiguration configuration, ISmsProvider smsProvider)
        {
            this.facebookProvider = facebookProvider;
            this.smsProvider = smsProvider;
            this.configuration = configuration;
            networkCredential = new NetworkCredential(configuration["Settings:Tvilio:accountSid"],
                configuration["Settings:Tvilio:authToken"], configuration["Settings:Tvilio:domain"]);
        }
        public async Task AddItemToFirebaseList(string subject, List<Data> dataDictionary, Data data, Credantials credantials, int index)
        {
            if (!dataDictionary.Any(x => x.Url == data.Url))
            {
                FirebaseClient client = GetFirebaseClient();
                await SendDataToFirebase(client, subject, data);
                if (index >= 3)
                 await smsProvider.Send(networkCredential, new MyLibraries.SMSMessage()
                    { Recipients = new List<MyLibraries.Recipient>() 
                    { new MyLibraries.Recipient(credantials.Phone,
                    $"new {credantials.Subject}: {data.Url}") }});
                client.Dispose();
            }
        }

        public async Task<Dictionary<string, Data>> GetDataFromFirebase(FirebaseClient firebaseClient, string subject)
        {
            Dictionary<string, Data> data = JsonConvert.DeserializeObject<Dictionary<string, Data>>(
                                            (await firebaseClient.GetAsync(subject)).Body);
            firebaseClient.Dispose();
            return data;
        }

        public FirebaseClient GetFirebaseClient() => new FirebaseClient(new FireSharp.Config.FirebaseConfig
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

        public async Task<string> Unsubscribe(Credantials credantials)
        {
            FireSharp.FirebaseClient firebaseClent = GetFirebaseClient();
            KeyValuePair<string, SubscriptionName> itemToUnsubscribe = (await isSubscribed(firebaseClent, credantials));
            // Remove a record to a subscription folder in the db 
            await firebaseClent.DeleteAsync($"{credantials.Phone}/subscriptions/{itemToUnsubscribe.Key}");
            // Remove db records
            await firebaseClent.DeleteAsync($"{credantials.Phone}/{itemToUnsubscribe.Value?.Name}");
            firebaseClent.Dispose();
            return "Unsubscribed";
        }

        public async Task<string> Subscribe(Credantials credantials)
        {
            string path = $"{credantials.Phone}/{credantials.Subject}";
            FireSharp.FirebaseClient firebaseClent = GetFirebaseClient();

            // Add a record to a subscription folder in the db 
            await SendDataToFirebase(firebaseClent, $"{credantials.Phone}/subscriptions", new SubscriptionName(credantials.Subject));


#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                //check if still subscribed
                CancellationTokenSource cancellationToken = new CancellationTokenSource();
                int index = 0;
                string isCanceled = (await isSubscribed(firebaseClent, credantials)).Value.Name;
                while (isCanceled is not null)
                {
                    if (isCanceled is null)
                        cancellationToken.Cancel();
                    index++;
                    int randomTime = new Random().Next(1, 10);
                    await populateFacebookDataAndUpdateDb(firebaseClent, path, credantials, index);
                    await Task.Delay(randomTime * 100000 , cancellationToken.Token);
                        if (cancellationToken.IsCancellationRequested)
                        {
                            Unsubscribe(credantials);
                            firebaseClent.Dispose();
                            break;
                        }
                }
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            return "Subscribed";
        }



        public async Task<bool> DeleteFileLocally(string fileName)
        {
            await Task.Yield();
            if (File.Exists(Path.Combine("Attachments", fileName)))
                File.Delete(Path.Combine("Attachments", fileName));
            return true;
        }

        public async Task<List<Uri>> UploadAttachmentsToFirebase()
        {
            FirebaseAuthLink firebaseAuthLink = await new FirebaseAuthProvider(
                new FirebaseConfig(configuration["Settings:Firebase:apiKey"])).SignInWithEmailAndPasswordAsync(
            configuration["Settings:Firebase:authEmail"], configuration["Settings:Firebase:authPassword"]);

            List<Uri> links = new List<Uri>();
            foreach (FileInfo file in new DirectoryInfo(@"Attachments").GetFiles())
            {
                string fileName = file.Name;
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes($"Attachments/{fileName}")))
                {
                    FirebaseStorageTask task = new FirebaseStorage(configuration["Settings:Firebase:bucket"], new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(firebaseAuthLink.FirebaseToken),
                    })
                        .Child("sms-attachments")
                        .Child(fileName)
                        .PutAsync(ms);

                    links.Add(new Uri(await task));
                }
            }

            return links;
        }


        private async Task populateFacebookDataAndUpdateDb(FirebaseClient firebaseClent, string path, Credantials credantials, 
             int index)
        {

            Dictionary<string, Data> OldDataFromFirebase = await GetDataFromFirebase(firebaseClent, path);

            List<Data> newDataFromFaceBook = await facebookProvider.GetDataFromFacebook(
                                             await facebookProvider.GetBrowserPage(), credantials.Subject);

           await addNewItemsToFirebase(newDataFromFaceBook, path, OldDataFromFirebase,credantials, index);

        }

        private async Task addNewItemsToFirebase(List<Data> newDataFromFaceBook, string path, 
            Dictionary<string, Data> OldDataFromFirebase, Credantials credantials, int index)
        {
            foreach (Data item in newDataFromFaceBook)
                await AddItemToFirebaseList(path,
                                    OldDataFromFirebase is null ? new List<Data>() :
                                    OldDataFromFirebase.Select(oldItem => oldItem.Value).ToList(),
                                    item,
                                    credantials,
                                    index);
        }

        private async Task<KeyValuePair<string, SubscriptionName>> isSubscribed(FirebaseClient firebaseClent, Credantials credantials)
        {
            KeyValuePair<string, SubscriptionName> responst = (await GetSubscriptions(firebaseClent, credantials.Phone))
                                                               .FirstOrDefault(x => x.Value?.Name == credantials.Subject);
            firebaseClent.Dispose();
            return responst;
        }

        private IFacebookProvider facebookProvider;
        private ISmsProvider smsProvider;
        private IConfiguration configuration;
        private NetworkCredential networkCredential;
    }
}
