using GoogleEmailApi.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GoogleEmailApi.Core.Services
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public class CredentialService
    {
        private readonly IMongoCollection<Credential> _creds;

        public CredentialService(IOptions<MongoSettings> opts)
        {
            var client = new MongoClient(opts.Value.ConnectionString);
            var db = client.GetDatabase(opts.Value.DatabaseName);
            _creds = db.GetCollection<Credential>("Credentials");
        }

        public Credential? Get(string key) =>
            _creds.Find(c => c.Key == key).FirstOrDefault();
    }
}
