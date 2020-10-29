using MessagingServiceApp.Data.Interfaces;

namespace MessagingServiceApp.Data
{
    public class MongoDatabaseSettings : IMongoDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string MessageInfosCollectionName { get; set; }
    }
}
