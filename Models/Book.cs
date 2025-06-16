using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace BookApi.Models;

public class Book
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] 
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public DateTime PublishedDate { get; set; }
}