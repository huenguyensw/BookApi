using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookApi.Models;
using BookApi.Services;

namespace BookApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;

        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetBooks()
        {
            var books = await _bookService.GetBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(string id)
        {
            var book = await _bookService.GetBookAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(Book book)
        {
            await _bookService.CreateBookAsync(book);
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(string id, Book updatedBook)
        {
            var existingBook = await _bookService.GetBookAsync(id);
            if (existingBook == null)
            {
                return NotFound();
            }
            updatedBook.Id = existingBook.Id;
            await _bookService.UpdateBookAsync(id, updatedBook);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(string id)
        {
            var existingBook = await _bookService.GetBookAsync(id);
            if (existingBook == null)
            {
                return NotFound();
            }
            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }
    }
}