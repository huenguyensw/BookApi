using Microsoft.AspNetCore.Mvc;
using BookApi.Models;
using BookApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace BookApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        private readonly QuoteService _quoteService;

        public QuotesController(QuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Quote>>> GetQuotes()
        {
            var quotes = await _quoteService.GetQuotesAsync();
            return Ok(quotes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Quote>> GetQuote(string id)
        {
            var quote = await _quoteService.GetQuoteAsync(id);
            if (quote == null)
            {
                return NotFound();
            }
            return Ok(quote);
        }

        [HttpPost]
        public async Task<ActionResult<Quote>> CreateQuote(Quote quote)
        {
            await _quoteService.CreateQuoteAsync(quote);
            return CreatedAtAction(nameof(GetQuote), new { id = quote.Id }, quote);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuote(string id, Quote updatedQuote)
        {
            var existingQuote = await _quoteService.GetQuoteAsync(id);
            if (existingQuote == null)
            {
                return NotFound();
            }
            updatedQuote.Id = existingQuote.Id;
            await _quoteService.UpdateQuoteAsync(id, updatedQuote);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuote(string id)
        {
            var existingQuote = await _quoteService.GetQuoteAsync(id);
            if (existingQuote == null)
            {
                return NotFound();
            }
            await _quoteService.DeleteQuoteAsync(id);
            return NoContent();
        }
    }
}