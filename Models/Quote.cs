using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace BookApi.Models;

public class Quote
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    
    public string Text { get; set; } = null!;
    public string Author { get; set; } = null!;
}
