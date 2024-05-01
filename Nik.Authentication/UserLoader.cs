namespace Nik.Authentication;

public sealed class UserLoader(AuthenticationDbContext dbContext) : IUserLoader
{
    public Task<User> LoadByNameAsync(string username)
    {
        var filter = Builders<User>.Filter.Eq(s => s.Username, username);
        return dbContext.Users.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        var filter = Builders<User>.Filter.Or(
            Builders<User>.Filter.Eq(user => user.Username, username),
            Builders<User>.Filter.Eq(user => user.Email, username),
            Builders<User>.Filter.Eq(user => user.Phone, username)
        );

        var users = await dbContext.Users.Find(filter).ToListAsync();
        return users.Count > 0;
    }
}