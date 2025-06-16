using BookApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookApi.Services
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _books;

        public BookService(IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(bookStoreDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName);
            _books = mongoDatabase.GetCollection<Book>(bookStoreDatabaseSettings.Value.BooksCollectionName);
        }

       public async Task<List<Book>> GetBooksAsync() =>
        await _books.Find(book => true).ToListAsync();

        public async Task<Book?> GetBookAsync(string id) =>
        await _books.Find(book => book.Id == id).FirstOrDefaultAsync();

        public async Task CreateBookAsync(Book book) =>
        await _books.InsertOneAsync(book);

        public async Task UpdateBookAsync(string id, Book updatedBook) =>
        await _books.ReplaceOneAsync(book => book.Id == id, updatedBook);

        public async Task DeleteBookAsync(string id) =>
        await _books.DeleteOneAsync(book => book.Id == id);
    }
}