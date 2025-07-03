using BookApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookApi.Services
{
    public class QuoteService
    {
        private readonly IMongoCollection<Quote> _quotes;

        public QuoteService(IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(bookStoreDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName);
            _quotes = mongoDatabase.GetCollection<Quote>(bookStoreDatabaseSettings.Value.QuotesCollectionName);
        }

        public async Task<List<Quote>> GetQuotesAsync() =>
            await _quotes.Find(quote => true).ToListAsync();

        public async Task<Quote?> GetQuoteAsync(string id) =>
            await _quotes.Find(quote => quote.Id == id).FirstOrDefaultAsync();

        public async Task CreateQuoteAsync(Quote quote) =>
            await _quotes.InsertOneAsync(quote);

        public async Task UpdateQuoteAsync(string id, Quote updatedQuote) =>
            await _quotes.ReplaceOneAsync(quote => quote.Id == id, updatedQuote);

        public async Task DeleteQuoteAsync(string id) =>
            await _quotes.DeleteOneAsync(quote => quote.Id == id);
    }
}

