using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace BookApi.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    
    public string Username { get; set; } = null!;
    
    public string PasswordHash { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
}
