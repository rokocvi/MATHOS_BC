using BootcampApp.Models;   
using Npgsql;
using static BootcampApp.Models.BookController;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text;
using BootcampApp.Repository.Common;
using BootcampApp.Service.Common;

namespace BootcampApp.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly string _connectionString;

        public BookRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Book>> GetAllBooksFromDbAsync(
    string? titleFilter = null,
    string? sortBy = null,
    string? sortDirection = "asc",
    int? page = null,
    int? pageSize = null)
        {
            var books = new Dictionary<int, Book>();

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var queryBuilder = new StringBuilder(@"
        SELECT 
            b.Id AS BookId, 
            b.Title, 
            b.AuthorId, 
            b.LibraryId,
            b.rating,
            a.Name AS AuthorName,
            g.Id AS GenreId,
            g.Name AS GenreName
        FROM Books b
        LEFT JOIN Authors a ON b.AuthorId = a.Id
        LEFT JOIN book_genres bg ON b.Id = bg.bookid
        LEFT JOIN genres g ON bg.genreid = g.id
    ");

            var whereClauses = new List<string>();
            var parameters = new List<NpgsqlParameter>();

            if (!string.IsNullOrWhiteSpace(titleFilter))
            {
                whereClauses.Add("LOWER(b.Title) LIKE LOWER(@titleFilter)");
                parameters.Add(new NpgsqlParameter("@titleFilter", $"%{titleFilter}%"));
            }

            if (whereClauses.Any())
            {
                queryBuilder.Append(" WHERE " + string.Join(" AND ", whereClauses));
            }

            var validSorts = new[] { "title", "authorid", "libraryid" };
            if (!string.IsNullOrWhiteSpace(sortBy) && validSorts.Contains(sortBy.ToLower()))
            {
                var dir = sortDirection?.ToLower() == "desc" ? "DESC" : "ASC";
                queryBuilder.Append($" ORDER BY b.{sortBy} {dir}");
            }

            if (page.HasValue && pageSize.HasValue)
            {
                int offset = (page.Value - 1) * pageSize.Value;
                queryBuilder.Append(" OFFSET @offset LIMIT @limit");
                parameters.Add(new NpgsqlParameter("@offset", offset));
                parameters.Add(new NpgsqlParameter("@limit", pageSize.Value));
            }

            await using var command = new NpgsqlCommand(queryBuilder.ToString(), connection);
            command.Parameters.AddRange(parameters.ToArray());

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var bookId = reader.GetInt32(reader.GetOrdinal("BookId"));

                if (!books.TryGetValue(bookId, out var book))
                {
                    book = new Book
                    {
                        Id = bookId,
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        AuthorId = reader.GetInt32(reader.GetOrdinal("AuthorId")),
                        LibraryId = reader.GetInt32(reader.GetOrdinal("LibraryId")),
                        Author = reader.IsDBNull(reader.GetOrdinal("AuthorName")) ? null : reader.GetString(reader.GetOrdinal("AuthorName")),
                        rating = reader.GetDouble(reader.GetOrdinal("rating")),
                        Genres = new List<Genre>()
                    };
                    books.Add(bookId, book);
                }

                if (!reader.IsDBNull(reader.GetOrdinal("GenreId")))
                {
                    var genre = new Genre
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("GenreId")),
                        Name = reader.GetString(reader.GetOrdinal("GenreName"))
                    };
                    book.Genres.Add(genre);
                }
            }

            return books.Values.ToList();
        }




        public async Task<Book?> GetBookFromDbAsync(int id)
        {
            var books = new Dictionary<int, Book>();

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
            SELECT 
                b.Id AS BookId, 
                b.Title, 
                b.AuthorId, 
                b.LibraryId,
                b.rating,
                a.Name AS AuthorName,
                g.Id AS GenreId,
                g.Name AS GenreName
            FROM Books b
            LEFT JOIN Authors a ON b.AuthorId = a.Id
            LEFT JOIN book_genres bg ON b.Id = bg.bookid
            LEFT JOIN genres g ON bg.genreid = g.id
            WHERE b.Id = @id";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var bookId = reader.GetInt32(reader.GetOrdinal("BookId"));

                if (!books.TryGetValue(bookId, out var book))
                {
                    book = new Book
                    {
                        Id = bookId,
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        AuthorId = reader.GetInt32(reader.GetOrdinal("AuthorId")),
                        LibraryId = reader.GetInt32(reader.GetOrdinal("LibraryId")),
                        Author = reader.IsDBNull(reader.GetOrdinal("AuthorName")) ? null : reader.GetString(reader.GetOrdinal("AuthorName")),
                        rating = reader.IsDBNull(reader.GetOrdinal("rating")) ? null : (double?)reader.GetDouble(reader.GetOrdinal("rating")),
                        Genres = new List<Genre>()
                    };
                    books.Add(bookId, book);
                }

                if (!reader.IsDBNull(reader.GetOrdinal("GenreId")))
                {
                    var genre = new Genre
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("GenreId")),
                        Name = reader.GetString(reader.GetOrdinal("GenreName"))
                    };
                    book.Genres.Add(genre);
                }
            }

            return books.Values.FirstOrDefault();
        }


        public async Task<Book> AddBookToDbAsync(Book book)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            await using (var checkAuthorCmd = new NpgsqlCommand("SELECT COUNT(*) FROM authors WHERE id = @id", connection))
            {
                checkAuthorCmd.Parameters.AddWithValue("@id", book.AuthorId);
                var authorExists = (long)await checkAuthorCmd.ExecuteScalarAsync();

                
                if (authorExists == 0)
                {
                    await using var insertAuthorCmd = new NpgsqlCommand("INSERT INTO authors (id, name) VALUES (@id, @name)", connection);
                    insertAuthorCmd.Parameters.AddWithValue("@id", book.AuthorId);
                    insertAuthorCmd.Parameters.AddWithValue("@name", book.Author);
                    await insertAuthorCmd.ExecuteNonQueryAsync();
                }
            }

            await using var insertBookCmd = new NpgsqlCommand(
                @"INSERT INTO books (title, authorid, author, libraryid) 
          VALUES (@title, @aid, @author, @lid)
          RETURNING id", connection);

            insertBookCmd.Parameters.AddWithValue("@title", book.Title);
            insertBookCmd.Parameters.AddWithValue("@aid", book.AuthorId);
            insertBookCmd.Parameters.AddWithValue("@author", book.Author);
            insertBookCmd.Parameters.AddWithValue("@lid", book.LibraryId);

            var idObj = await insertBookCmd.ExecuteScalarAsync();
            book.Id = Convert.ToInt32(idObj);

            return book;
        }



        public async Task UpdateBookInDbAsync(int id, Book book)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            
            await using (var checkAuthorCmd = new NpgsqlCommand("SELECT name FROM authors WHERE id = @id", connection))
            {
                checkAuthorCmd.Parameters.AddWithValue("@id", book.AuthorId);
                var result = await checkAuthorCmd.ExecuteScalarAsync();

                if (result == null)
                {
                    
                    await using var insertAuthorCmd = new NpgsqlCommand("INSERT INTO authors (id, name) VALUES (@id, @name)", connection);
                    insertAuthorCmd.Parameters.AddWithValue("@id", book.AuthorId);
                    insertAuthorCmd.Parameters.AddWithValue("@name", book.Author);
                    await insertAuthorCmd.ExecuteNonQueryAsync();
                }
                else
                {
                   
                    var existingName = result.ToString();
                    if (existingName != book.Author)
                    {
                        await using var updateAuthorCmd = new NpgsqlCommand("UPDATE authors SET name = @name WHERE id = @id", connection);
                        updateAuthorCmd.Parameters.AddWithValue("@name", book.Author);
                        updateAuthorCmd.Parameters.AddWithValue("@id", book.AuthorId);
                        await updateAuthorCmd.ExecuteNonQueryAsync();
                    }
                }
            }

            
            await using var command = new NpgsqlCommand(
                @"UPDATE books 
          SET title = @title, authorid = @aid, author = @aname, libraryid = @lid ,rating = @rating
          WHERE id = @id", connection);

            command.Parameters.AddWithValue("@title", book.Title);
            command.Parameters.AddWithValue("@aid", book.AuthorId);
            command.Parameters.AddWithValue("@aname", book.Author);
            command.Parameters.AddWithValue("@lid", book.LibraryId);
            command.Parameters.AddWithValue("@rating", book.rating);
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }


        public async Task DeleteBookFromDbAsync(int id)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
           await connection.OpenAsync();

            await using var command = new NpgsqlCommand("DELETE FROM books WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Book>> GetBooksByAuthorIdAsync(int authorId)
        {
            var books = new List<Book>();

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"
            SELECT 
                b.Id AS BookId, 
                b.Title, 
                b.AuthorId, 
                b.LibraryId,
                a.Id AS AuthorDbId, 
                a.Name AS AuthorName
            FROM Books b
            LEFT JOIN Authors a ON b.AuthorId = a.Id
            WHERE b.AuthorId = @authorId";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@authorId", authorId);

            await using var reader = await command.ExecuteReaderAsync();

            // Dobijamo indekse kolona koristeći alias-e
            var bookIdIndex = reader.GetOrdinal("BookId");
            var titleIndex = reader.GetOrdinal("Title");
            var authorIdIndex = reader.GetOrdinal("AuthorId");
            var libraryIdIndex = reader.GetOrdinal("LibraryId");
            var authorNameIndex = reader.GetOrdinal("AuthorName");

            while ( await reader.ReadAsync())
            {
                var book = new Book
                {
                    Id = reader.GetInt32(bookIdIndex),
                    Title = reader.GetString(titleIndex),
                    AuthorId = reader.GetInt32(authorIdIndex),
                    Author = reader.GetString(5),
                    LibraryId = reader.GetInt32(libraryIdIndex)   
                };

                books.Add(book);
            }

            return books;
        }
        public async Task<List<Book>> GetBooksByLibraryIdAsync(int libraryId)
        {
            var books = new List<Book>();

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // 1. Prvo dohvatimo sve knjige iz biblioteke
            var bookQuery = @"
        SELECT Id, Title, AuthorId, Author, LibraryId, Rating
        FROM books
        WHERE libraryid = @lid";

            await using var bookCommand = new NpgsqlCommand(bookQuery, connection);
            bookCommand.Parameters.AddWithValue("@lid", libraryId);

            var bookMap = new Dictionary<int, Book>();

            await using var reader = await bookCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var book = new Book
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    AuthorId = reader.GetInt32(2),
                    Author = reader.GetString(3),
                    LibraryId = reader.GetInt32(4),
                    rating = reader.IsDBNull(5) ? 0.0 : reader.GetDouble(5),
                    Genres = new List<Genre>() // inicijaliziraj prazno
                };

                books.Add(book);
                bookMap[book.Id] = book;
            }

            await reader.CloseAsync();

            // 2. Sada dohvatimo sve žanrove za te knjige
            if (bookMap.Count > 0)
            {
                var bookIds = string.Join(",", bookMap.Keys);
                var genreQuery = $@"
            SELECT bg.bookid, g.id, g.name
            FROM book_genres bg
            JOIN genres g ON g.id = bg.genreid
            WHERE bg.bookid IN ({bookIds})";

                await using var genreCommand = new NpgsqlCommand(genreQuery, connection);
                await using var genreReader = await genreCommand.ExecuteReaderAsync();

                while (await genreReader.ReadAsync())
                {
                    var bookId = genreReader.GetInt32(0);
                    var genre = new Genre
                    {
                        Id = genreReader.GetInt32(1),
                        Name = genreReader.GetString(2)
                    };

                    if (bookMap.TryGetValue(bookId, out var book))
                    {
                        book.Genres.Add(genre);
                    }
                }
            }

            return books;
        }

        public async Task<List<Genre>> GetGenresByBookIdAsync(
            int bookId,
            string? nameFilter = null,
            string? sortBy = null,
            string? sortDirection = "asc",
            int? page = null,
            int? pageSize = null)
        {

            var genres = new List<Genre>();
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = new StringBuilder(@"
                        SELECT 
                          g.id        AS GenreId,
                          g.name      AS GenreName
                        FROM genres g
                        JOIN book_genres bg ON g.id = bg.genreid
                        WHERE bg.bookid = @bookId
                    ");

            // 1) Filtering po imenu žanra
            var whereClauses = new List<string>();
            var parameters = new List<NpgsqlParameter>
    {
        new NpgsqlParameter("@bookId", bookId)
    };

            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                whereClauses.Add("LOWER(g.name) LIKE LOWER(@nameFilter)");
                parameters.Add(new NpgsqlParameter("@nameFilter", $"%{nameFilter}%"));
            }

            if (whereClauses.Any())
                sql.Append(" AND " + string.Join(" AND ", whereClauses));

            // 2) Sorting
            var validSorts = new[] { "GenreId", "GenreName" };
            if (!string.IsNullOrWhiteSpace(sortBy) && validSorts.Contains(sortBy))
            {
                var dir = sortDirection?.ToLower() == "desc" ? "DESC" : "ASC";
                sql.Append($" ORDER BY {sortBy} {dir}");
            }

            // 3) Paging
            if (page.HasValue && pageSize.HasValue)
            {
                int offset = (page.Value - 1) * pageSize.Value;
                sql.Append(" OFFSET @offset LIMIT @limit");
                parameters.Add(new NpgsqlParameter("@offset", offset));
                parameters.Add(new NpgsqlParameter("@limit", pageSize.Value));
            }

            await using var cmd = new NpgsqlCommand(sql.ToString(), conn);
            cmd.Parameters.AddRange(parameters.ToArray());  

            await using var reader = await cmd.ExecuteReaderAsync();

            var idIndex = reader.GetOrdinal("GenreId");
            var nameIndex = reader.GetOrdinal("GenreName");

            while (await reader.ReadAsync())
            {
                genres.Add(new Genre
                {
                    Id = reader.GetInt32(idIndex),
                    Name = reader.GetString(nameIndex)
                });
            }

            return genres;
        }


        public async Task AddGenresToBookAsync(int bookId, IEnumerable<int> genreIds)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = "INSERT INTO book_genres (bookid, genreid) VALUES (@bookid, @genreid) ON CONFLICT DO NOTHING";
            await using var cmd = new NpgsqlCommand( sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@bookId", bookId));
            var genreParam = cmd.Parameters.Add(new NpgsqlParameter("@genreId", DbType.Int32));

            foreach (var gid in genreIds)
            {
                genreParam.Value = gid;
                await cmd.ExecuteNonQueryAsync();
            }
        }

       
    }
}
