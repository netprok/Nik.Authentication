namespace Nik.Authentication.Models.Db;

public class AuthenticationDbContext : DbContext
{
    private const string RegistrationCollectionName = "registrations";
    private const string UserCollectionName = "users";
    private readonly IMongoDatabase _mongoDatabase;

    public AuthenticationDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _mongoDatabase = client.GetDatabase(databaseName);

        BsonSerializer.RegisterSerializer(new BsonEnumSerializer<EValidationMethod>());
        BsonSerializer.RegisterSerializer(new BsonEnumSerializer<EValidationStatus>());

        CreateIndices();
    }

    public IMongoCollection<Registration> Registrations => _mongoDatabase.GetCollection<Registration>(RegistrationCollectionName);
    public IMongoCollection<User> Users => _mongoDatabase.GetCollection<User>(UserCollectionName);

    public async Task<bool> UpdateAsync(User user) =>
        (await Users.ReplaceOneAsync(doc => doc.Id == user.Id, user)).IsAcknowledged;

    public async Task<bool> UpdateAsync(Registration registration) =>
        (await Registrations.ReplaceOneAsync(doc => doc.Id == registration.Id, registration)).IsAcknowledged;

    private void CreateIndices()
    {
        var registrationIndexModel = new CreateIndexModel<Registration>(
            Builders<Registration>.IndexKeys.Ascending(registration => registration.Username));
        Registrations.Indexes.CreateOne(registrationIndexModel);

        var userIndexModel = new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(user => user.Username),
            new CreateIndexOptions { Unique = true });
        Users.Indexes.CreateOne(userIndexModel);
    }
}