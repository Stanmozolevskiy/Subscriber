using DataModels;
using DataProviderInterfaces;
using FireSharp;
using FireSharp.Config;
using FireSharp.Response;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseProvider
{
    public class Provider : IFirebaseProvider
    {
        public async Task AddItemToFirebaseList(string subject, List<Data> dataDictionary, Data data, IConfiguration configuration)
        {
            if(!dataDictionary.Any(x => x.Url == data.Url))
             await SendDataToFirebase(GetFirebaseClient(configuration), subject, data);
        }

        public async Task<Dictionary<string, Data>> GetDataFromFirebase(FirebaseClient firebaseClient, string subject)
        {
            Dictionary<string, Data> data = JsonConvert.DeserializeObject<Dictionary<string, Data>>((
                                                    await firebaseClient.GetAsync(subject)).Body);
            firebaseClient.Dispose();
            return data;
        }

        public FirebaseClient GetFirebaseClient(IConfiguration configuration) => new FirebaseClient(new FirebaseConfig
        {
            AuthSecret = configuration["Firebase:AuthSecret"],
            BasePath = configuration["Firebase:BasePath"]
        });

        public async Task<PushResponse> Register(FirebaseClient firebaseClient, string phone)
        {
            PushResponse data = await firebaseClient.PushAsync(phone, phone);
            firebaseClient.Dispose();
            return data;
        }

        public async Task<PushResponse> SendDataToFirebase<T>(FirebaseClient firebaseClient, string subject, T data)
        {
            PushResponse response = await firebaseClient.PushAsync(subject, data);
            firebaseClient.Dispose();
            return response;
        }
 
    }
}
