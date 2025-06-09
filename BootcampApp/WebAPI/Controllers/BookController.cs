using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.Linq;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class BookController : ControllerBase
    {
        public static List<Book> books = new List<Book>
        {
            new Book { Id = 1, Title = "Na Drini ćuprija", Author = "Ivo Andrić", AuthorId = 1 },
            new Book { Id = 2, Title = "Zločin i kazna", Author = "Fjodor Dostojevski", AuthorId = 2 },
            new Book { Id = 3, Title = "Rat i mir", Author = "Lav Tolstoj", AuthorId = 3 },
            new Book { Id = 4, Title = "Mali princ", Author = "Antoine de Saint-Exupéry", AuthorId = 4 },
            new Book { Id = 5, Title = "Gorski vijenac", Author = "Petar II Petrović Njegoš", AuthorId = 5 },
            new Book { Id = 6, Title = "Ana Karenjina", Author = "Lav Tolstoj", AuthorId = 3 },
            new Book { Id = 7, Title = "1984", Author = "George Orwell", AuthorId = 6 },
            new Book { Id = 8, Title = "Ponos i predrasude", Author = "Jane Austen", AuthorId = 7 },
            new Book { Id = 9, Title = "Lovac u žitu", Author = "J.D. Salinger", AuthorId = 8 },
            new Book { Id = 10, Title = "Braća Karamazovi", Author = "Fjodor Dostojevski", AuthorId = 2 }
        };

        private readonly string _connectionString = "Host=localhost;Port=5432;Username=postgres;Password=UtwG5EC4;Database=postgres";

        [HttpGet("db")]
        public ActionResult<List<Book>> GetAllBooksFromDb()
        {
            var books = new List<Book>();


            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("SELECT id, title, author, authorid FROM books", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            books.Add(new Book
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Author = reader.GetString(2),
                                AuthorId = reader.GetInt32(3)
                            });
                        }
                    }
                }

                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška prilikom dohvaćanja podataka iz baze.", error = ex.Message });
            }

            
        }


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

        [HttpPost("multibulk")]
        public ActionResult AddMultipleListsOfBooks([FromBody] List<List<Book>> bookGroups)
        {
            if (bookGroups == null || !bookGroups.Any())
                return BadRequest("Nema poslanih listi knjiga.");

            var errors = new List<string>();
            int addedCount = 0;
            int nextId = books.Any() ? books.Max(b => b.Id) + 1 : 1;

            
            var existingTitles = new HashSet<string>(books.Select(b => b.Title.Trim().ToLower()));

        
            var titlesInRequest = new HashSet<string>();

            foreach (var group in bookGroups)
            {
                if (group == null || !group.Any())
                {
                    errors.Add("Jedna od listi knjiga je prazna.");
                    continue;
                }

                foreach (var book in group)
                {
                    if (string.IsNullOrWhiteSpace(book.Title))
                    {
                        errors.Add("Knjiga s praznim naslovom nije dodana.");
                        continue;
                    }

                    var normalizedTitle = book.Title.Trim().ToLower();

                    if (string.IsNullOrWhiteSpace(book.Author))
                    {
                        errors.Add($"Autor nije naveden za knjigu '{book.Title}'.");
                        continue;
                    }

                    if (existingTitles.Contains(normalizedTitle))
                    {
                        errors.Add($"Knjiga sa naslovom '{book.Title}' već postoji u bazi.");
                        continue;
                    }

                    if (titlesInRequest.Contains(normalizedTitle))
                    {
                        errors.Add($"Knjiga sa naslovom '{book.Title}' je duplikat unutar zahtjeva.");
                        continue;
                    }

                   
                    titlesInRequest.Add(normalizedTitle);

                    book.Id = nextId++;
                    books.Add(book);
                    addedCount++;
                }
            }

            if (errors.Any())
            {
                return BadRequest(new
                {
                    poruka = "Dio knjiga nije dodan zbog grešaka.",
                    dodano = addedCount,
                    greske = errors
                });
            }

            return Ok(new
            {
                poruka = "Sve knjige su uspješno dodane.",
                ukupno = addedCount
            });
        }

        [HttpGet("db/{id}")]
        public ActionResult<Book> GetBookFromDb(int id)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("SELECT id, title, author, authorid FROM books WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("id", id);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var book = new Book
                                {
                                    Id = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Author = reader.GetString(2),
                                    AuthorId = reader.GetInt32(3)
                                };

                                return Ok(book);
                            }
                            else
                            {
                                return NotFound("Knjiga nije pronađena.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost("db")]
        public ActionResult<Book> AddBookToDb([FromBody] Book newBook)
        {
            if (newBook == null || string.IsNullOrWhiteSpace(newBook.Title) || string.IsNullOrWhiteSpace(newBook.Author))
            {
                return BadRequest("Title and Author are required.");
            }

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                using var cmd = new NpgsqlCommand(
                    "INSERT INTO books (title, author, authorid) VALUES (@title, @author, @authorid) RETURNING id",
                    conn);
                cmd.Parameters.AddWithValue("title", newBook.Title);
                cmd.Parameters.AddWithValue("author", newBook.Author);
                cmd.Parameters.AddWithValue("authorid", newBook.AuthorId);

                var result = cmd.ExecuteScalar();

                if (result != null)
                {
                    newBook.Id = Convert.ToInt32(result);
                    return CreatedAtAction(nameof(GetBookFromDb), new { id = newBook.Id }, newBook);
                }

                return StatusCode(500, "Insert failed.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("db/{id}")]
        public IActionResult UpdateBookInDb(int id, [FromBody] Book updatedBook)
        {
            if (updatedBook == null || string.IsNullOrWhiteSpace(updatedBook.Title) || string.IsNullOrWhiteSpace(updatedBook.Author))
            {
                return BadRequest("Title and Author are required.");
            }

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                using var cmd = new NpgsqlCommand(
                    "UPDATE books SET title = @title, author = @author, authorid = @authorid WHERE id = @id",
                    conn);

                cmd.Parameters.AddWithValue("title", updatedBook.Title);
                cmd.Parameters.AddWithValue("author", updatedBook.Author);
                cmd.Parameters.AddWithValue("authorid", updatedBook.AuthorId);
                cmd.Parameters.AddWithValue("id", id);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound("Book not found.");
                }

                return NoContent(); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("db/{id}")]
        public IActionResult DeleteBookFromDb(int id)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();


                using var cmd = new NpgsqlCommand("DELETE FROM books where id = @id", conn);
                cmd.Parameters.AddWithValue("id", id);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound("Book not found.");
                }

                return NoContent(); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        
        }

        [HttpGet("authors/under50")]
        public List<Author> GetAuthorsUnder50WithBooks()
        {
            var authors = new List<Author>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                string sql = @"
            SELECT a.id as AuthorId, a.name, a.age, b.id as BookId, b.title
            FROM authors a
            LEFT JOIN books b ON a.id = b.authorid
            WHERE a.age < 60;
        ";

                using var cmd = new NpgsqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();

                var authorDict = new Dictionary<int, Author>();

                while (reader.Read())
                {
                    int authorId = reader.GetInt32(reader.GetOrdinal("AuthorId"));

                    if (!authorDict.TryGetValue(authorId, out var author))
                    {
                        author = new Author
                        {
                            Id = authorId,
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Age = reader.GetInt32(reader.GetOrdinal("age")),
                            Books = new List<Book>()
                        };
                        authorDict[authorId] = author;
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("BookId")))
                    {
                        var book = new Book
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("BookId")),
                            Title = reader.GetString(reader.GetOrdinal("title")),
                            Author = reader.GetString(reader.GetOrdinal("name")),
                            AuthorId = authorId
                        };
                        author.Books.Add(book);
                    }
                }

                authors = authorDict.Values.ToList();
            }

            return authors;
        }


    }
}
