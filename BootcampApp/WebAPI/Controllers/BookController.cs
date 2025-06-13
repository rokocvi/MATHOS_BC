using Microsoft.AspNetCore.Mvc;
using BootcampApp.Models;
using BootcampApp.Service;
using System.Collections.Generic;
using BootcampApp.Service.Common;
using BootcampApp.Repository.Common;


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
        public async Task<ActionResult<List<Book>>> GetBooks(string? titleFilter = null,
            string? sortBy = null,
            string? sortDirection = "asc",
            int? page = null,
            int? pageSize = null)
        {
            var books = await _bookService.GetAllBooksAsync(titleFilter, sortBy, sortDirection, page, pageSize);
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
        public async Task<IActionResult> UpdateBookAsync(int id, [FromBody] Book updatedBook)
        {
            var existingBook = await _bookService.GetBookAsync(id);
            if (existingBook == null)
                return NotFound();

            await _bookService.UpdateBookAsync(id, updatedBook);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async  Task<IActionResult> DeleteBookAsync(int id)
        {
            var existingBook = await _bookService.GetBookAsync(id);
            if (existingBook == null)
                return NotFound();

           await _bookService.DeleteBookAsync(id);
            return NoContent();
        }

        [HttpGet("author/{authorId}/books")]
        public async Task<ActionResult<List<Book>>> GetBooksByAuthorAsync(int authorId, string sortBy, string sortDirection, double? minRating)
        {
            var books = await _bookService.GetBooksByAuthorAsync(authorId, sortBy, sortDirection,minRating);
            if (books == null || books.Count == 0)
                return NotFound($"No books found for author with ID {authorId}.");

            return Ok(books);
        }

        [HttpGet("library/{libraryId}/books")]
        public async Task<ActionResult<List<Book>>> GetBooksByLibraryAsync(int libraryId)
        {
            var books = await _bookService.GetBooksByLibraryAsync(libraryId);
            return Ok(books);
        }

        [HttpGet("{bookId}/genres")]
        public async Task<ActionResult<List<BootcampApp.Models.Genre>>> GetGenresByBookAsync(int bookId, string? nameFilter = null, string? sortBy = null, string? sortDirection = "asc",
            int? page = null,
            int? pageSize = null)
        {
            var genres = await _bookService.GetGenresByBookAsync(bookId, nameFilter, sortBy, sortDirection, page, pageSize);
            return Ok(genres);
        }

        [HttpPost("{id}/genres")]
        public async Task<IActionResult> AddGenres(int id, [FromBody] List<int> genreIds)
        {
            await _bookService.AddGenresToBookAsync(id, genreIds);
            return NoContent();
        }

        [HttpGet("{userid}/loans")]

        public async Task<ActionResult<List<Loan>>> GetLoans(int userid)
        {
            var loans = await _bookService.GetLoansByUserIdAsync(userid);
            return Ok(loans);
         
        }
    }
}
