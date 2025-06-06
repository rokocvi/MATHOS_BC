using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private static List<Book> books = new List<Book>
        {
            new Book { Id = 1, Title = "Na Drini ćuprija", Author = "Ivo Andrić" },
            new Book { Id = 2, Title = "Zločin i kazna", Author = "Fjodor Dostojevski" },
            new Book { Id = 3, Title = "Rat i mir", Author = "Lav Tolstoj" },
            new Book { Id = 4, Title = "Mali princ", Author = "Antoine de Saint-Exupéry" },
            new Book { Id = 5, Title = "Gorski vijenac", Author = "Petar II Petrović Njegoš" },
            new Book { Id = 6, Title = "Ana Karenjina", Author = "Lav Tolstoj" },
            new Book { Id = 7, Title = "1984", Author = "George Orwell" },
            new Book { Id = 8, Title = "Ponos i predrasude", Author = "Jane Austen" },
            new Book { Id = 9, Title = "Lovac u žitu", Author = "J.D. Salinger" },
            new Book { Id = 10, Title = "Braća Karamazovi", Author = "Fjodor Dostojevski" }
        };


        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetBooks()
        {
            return Ok(books);
        }

        [HttpGet("{id}")]
        public ActionResult<Book> GetBook(int id)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return NotFound();
            return Ok(book);
        }

        [HttpPost]

        public ActionResult<Book> AddBook([FromBody] Book newBook)
        {   
            newBook.Id = books.Count > 0 ? books.Max(b => b.Id) + 1 : 1;
            books.Add(newBook);
            return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
        }

        [HttpDelete("{id}")]

        public IActionResult DeleteBook(int id)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return NotFound();
            books.Remove(book);

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, [FromBody] Book updatedBook)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return NotFound();

            
            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;

            return Ok(book);
        }

        [HttpGet("search")]

        public ActionResult <List<Book>> SearchBook([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query parameter is required. ");
            }

            var results = books
                          .Where(b => b.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                          || b.Author.Contains(query, StringComparison.OrdinalIgnoreCase))
                           .ToList();

            if(results.Count == 0)
            {
                return NotFound("No books matching from query");
            }

            return Ok(results);

        }

        [HttpGet("count")]

        public ActionResult<int> Count()
        {
            return Ok(books.Count());
        }

        [HttpGet("random")]

        public ActionResult<Book> GetRandomBook()
        {
            if (!books.Any())
            {
                return NotFound("No books available");
            }

            var random = new Random();
            var book = books[random.Next(books.Count)];

            return Ok(book);
        }

        [HttpGet("latest")]
        public ActionResult<Book> GetLatestBook()
        {
            if (!books.Any())
                return NotFound("No books available.");

            var latest = books.OrderByDescending(b => b.Id).First();
            return Ok(latest);
             
        }
    }
}
