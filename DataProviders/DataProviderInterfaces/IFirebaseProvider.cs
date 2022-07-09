using DataModels;
using Firebase.Auth;
using FireSharp;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DataProviderInterfaces
{
    public interface IFirebaseProvider
    {
        FirebaseClient GetFirebaseClient();
        Task<Dictionary<string, Data>> GetDataFromFirebase(FirebaseClient firebaseClient, string subject);
        Task<PushResponse> SendDataToFirebase<T>(FirebaseClient firebaseClient, string subject, T data);
        Task<string> Register(FirebaseClient firebaseClient, string phone);
        Task<Dictionary<string, SubscriptionName>> GetSubscriptions(FirebaseClient firebaseClient, string phone);
        Task AddItemToFirebaseList(string subject, List<Data> dataDictionary, Data data, Credantials credantials, int index);
        Task<string> Subscribe(Credantials credantials);
        Task<string> Unsubscribe(Credantials credantials);


        Task<bool> DeleteFileLocally(string fileName);
        Task<List<Uri>> UploadAttachmentsToFirebase();

    }
}
