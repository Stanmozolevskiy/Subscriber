using DataModels;
using FireSharp;
using FireSharp.Response;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataProviderInterfaces
{
    public interface IFirebaseProvider
    {
        FirebaseClient GetFirebaseClient(IConfiguration configuration);
        Task<Dictionary<string, Data>> GetDataFromFirebase(FirebaseClient firebaseClient, string subject);
        Task<PushResponse> SendDataToFirebase<T>(FirebaseClient firebaseClient, string subject, T data);
        Task<PushResponse> Register(FirebaseClient firebaseClient, string phone);
        Task AddItemToFirebaseList(string subject, List<Data> dataDictionary, Data data, IConfiguration configuration);
    }
}
