using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore.V1;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using Grpc.Auth;

namespace Tailwind.Traders.Product.Api.Infrastructure
{
    public static class GcpHelper
    {
        public static FirestoreDb CreateDb(FireStoreKeys firestoreSettings)
        {
            string firestoreServiceKeyJson = JsonConvert.SerializeObject(firestoreSettings);
            var cred = GoogleCredential.FromJson(firestoreServiceKeyJson);

            var clientBuilder = new FirestoreClientBuilder
            {
                ChannelCredentials = cred.ToChannelCredentials()
            };

            FirestoreClient client = clientBuilder.Build();
            var db = FirestoreDb.Create(firestoreSettings.ProjectId, client);
            return db;
        }

    }
}
