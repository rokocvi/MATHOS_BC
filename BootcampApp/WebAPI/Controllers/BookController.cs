using Microsoft.AspNetCore.Mvc;
using BootcampApp.Models;
using BootcampApp.Service;
using System.Collections.Generic;
using BootcampApp.Service.Interfaces;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _bookService.GetBookAsync(id);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> AddBookAsync([FromBody] Book newBook)
        {
            if (newBook == null || string.IsNullOrWhiteSpace(newBook.Title) || string.IsNullOrWhiteSpace(newBook.Author))
                return BadRequest("Title and Author are required.");

            var addedBook = await _bookService.AddBookAsync(newBook);
            return CreatedAtAction(nameof(GetBook), new { id = addedBook.Id }, addedBook);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, [FromBody] Book updatedBook)
        {
            var existingBook = _bookService.GetBookAsync(id);
            if (existingBook == null)
                return NotFound();

            _bookService.UpdateBook(id, updatedBook);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            var existingBook = _bookService.GetBookAsync(id);
            if (existingBook == null)
                return NotFound();

            _bookService.DeleteBook(id);
            return NoContent();
        }

        [HttpGet("author/{authorId}/books")]
        public ActionResult<List<Book>> GetBooksByAuthor(int authorId)
        {
            var books = _bookService.GetBooksByAuthor(authorId);
            if (books == null || books.Count == 0)
                return NotFound($"No books found for author with ID {authorId}.");

            return Ok(books);
        }

        [HttpGet("library/{libraryId}/books")]
        public ActionResult<List<Book>> GetBooksByLibrary(int libraryId)
        {
            var books = _bookService.GetBooksByLibrary(libraryId);
            return Ok(books);
        }

        [HttpGet("genres/bybook/{bookId}")]
        public ActionResult<List<BootcampApp.Models.Genre>> GetGenresByBook(int bookId)
        {
            var genres = _bookService.GetGenresByBook(bookId);
            return Ok(genres);
        }
    }
}
