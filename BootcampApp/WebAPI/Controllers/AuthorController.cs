using Microsoft.AspNetCore.Mvc;
using BootcampApp.Models;
using BootcampApp.Service;
using System.Collections.Generic;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public ActionResult<List<Author>> GetAll()
        {
            return Ok(_authorService.GetAllAuthors());
        }

        [HttpGet("{id}")]
        public ActionResult<Author> GetById(int id)
        {
            var author = _authorService.GetAuthorByIdAsync(id);
            if (author == null)
                return NotFound();
            return Ok(author);
        }

        [HttpPost]
        public ActionResult<Author> Create([FromBody] Author author)
        {
            var created = _authorService.CreateAuthorAsync(author);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Author author)
        {
            var updated = _authorService.UpdateAuthorAsync(id, author);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _authorService.DeleteAuthor(id);
            if (!deleted)
                return BadRequest("Author cannot be deleted because it has related books.");
            return NoContent();
        }

        [HttpGet("{authorId}/books")]
        public ActionResult<List<Book>> GetBooksByAuthor(int authorId)
        {
            var books = _authorService.GetBooksByAuthor(authorId);
            if (books == null || books.Count == 0)
                return NotFound();
            return Ok(books);
        }
    }
}
