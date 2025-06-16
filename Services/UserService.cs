using BookApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookApi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IOptions<UserManagementSettings> UserManagementSettings)
        {
            var mongoClient = new MongoClient(UserManagementSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(UserManagementSettings.Value.DatabaseName);
            _users = mongoDatabase.GetCollection<User>(UserManagementSettings.Value.UsersCollectionName);
        }

        public async Task<List<User>> GetUsersAsync() =>
        await _users.Find(user => true).ToListAsync();

        public async Task<User?> GetUserByEmailAsync(string email) =>
        await _users.Find(user => user.Email == email).FirstOrDefaultAsync();   

        public async Task<User?> GetUserAsync(string id) =>
        await _users.Find(user => user.Id == id).FirstOrDefaultAsync();

        public async Task CreateUserAsync(User user) =>
        await _users.InsertOneAsync(user);

        public async Task UpdateUserAsync(string id, User updatedUser) =>
        await _users.ReplaceOneAsync(user => user.Id == id, updatedUser);

        public async Task DeleteUserAsync(string id) =>
        await _users.DeleteOneAsync(user => user.Id == id);

    }  
    
}