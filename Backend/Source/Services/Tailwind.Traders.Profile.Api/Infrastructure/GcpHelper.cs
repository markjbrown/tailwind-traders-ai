using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore.V1;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using Grpc.Auth;

namespace Tailwind.Traders.Profile.Api.Infrastructure
{
    public static class GcpHelper
    {
        public static FirestoreDb CreateDb(GcpFirestoreConfig firestoreSettings)
        {
            var clientBuilder = new FirestoreClientBuilder();
            FirestoreClient client = clientBuilder.Build();
            var db = FirestoreDb.Create(null, client);
            return db;
        }

    }
}
