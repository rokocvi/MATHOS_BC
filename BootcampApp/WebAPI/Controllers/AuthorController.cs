using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private static List<Author> authors = new List<Author>
        {
            new Author { Id = 1, Name = "Ivo Andrić" },
            new Author { Id = 2, Name = "Fjodor Dostojevski" }
        };

        // GET: api/author
        [HttpGet]
        public ActionResult<List<Author>> GetAll()
        {
            return Ok(authors);
        }

        // GET: api/author/5
        [HttpGet("{id}")]
        public ActionResult<Author> GetById(int id)
        {
            var author = authors.FirstOrDefault(a => a.Id == id);
            if (author == null)
                return NotFound($"Autor sa ID {id} nije pronađen.");
            return Ok(author);
        }

        // POST: api/author
        [HttpPost]
        public ActionResult<Author> Create([FromBody] Author author)
        {
            if (string.IsNullOrWhiteSpace(author.Name))
                return BadRequest("Ime autora ne smije biti prazno.");

            if (authors.Any(a => a.Name.ToLower() == author.Name.ToLower()))
                return Conflict("Autor sa tim imenom već postoji.");

            int newId = authors.Any() ? authors.Max(a => a.Id) + 1 : 1;
            author.Id = newId;
            authors.Add(author);

            return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
        }

        // PUT: api/author/5
        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Author updatedAuthor)
        {
            var existingAuthor = authors.FirstOrDefault(a => a.Id == id);
            if (existingAuthor == null)
                return NotFound($"Autor sa ID {id} nije pronađen.");

            if (string.IsNullOrWhiteSpace(updatedAuthor.Name))
                return BadRequest("Ime autora ne smije biti prazno.");

            if (authors.Any(a => a.Id != id && a.Name.ToLower() == updatedAuthor.Name.ToLower()))
                return Conflict("Drugi autor sa tim imenom već postoji.");

            existingAuthor.Name = updatedAuthor.Name;

            return NoContent();
        }

        // DELETE: api/author/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var author = authors.FirstOrDefault(a => a.Id == id);
            if (author == null)
                return NotFound($"Autor sa ID {id} nije pronađen.");

            // Opcionalno: provjera da nema knjiga koje koriste ovog autora
            var hasBooks = BookController.books.Any(b => b.AuthorId == id);
            if (hasBooks)
                return BadRequest("Ne možete obrisati autora koji ima povezane knjige.");

            authors.Remove(author);
            return NoContent();
        }

        [HttpGet("byauthor/{authorId}")]
        public ActionResult<List<Book>> GetBooksByAuthor(int authorId)
        {
            var booksByAuthor = BookController.books.Where(b => b.AuthorId == authorId).ToList();

            if (!booksByAuthor.Any())
                return NotFound($"Nema knjiga za autora sa ID {authorId}.");

            return Ok(booksByAuthor);
        }
    }
}
