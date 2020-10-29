using System;
namespace MessagingServiceApp.Data.Interfaces
{
    public interface IMongoDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string MessageInfosCollectionName { get; set; }
    }
}
