using BootcampApp.Models;
using BootcampApp.Repository.Interfaces;
using Npgsql;
using static BootcampApp.Models.BookController;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BootcampApp.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly string _connectionString;

        public BookRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Book> GetAllBooksFromDb()
        {
            var books = new List<Book>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            // Ispravljeni SQL: bez WHERE i s aliasima
            string query = @"
        SELECT 
            b.Id AS BookId, 
            b.Title, 
            b.AuthorId, 
            b.LibraryId,
            a.Id AS AuthorDbId, 
            a.Name AS AuthorName
        FROM Books b
        LEFT JOIN Authors a ON b.AuthorId = a.Id";

            using var command = new NpgsqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            // Indeksi prema aliasima
            var bookIdIndex = reader.GetOrdinal("BookId");
            var titleIndex = reader.GetOrdinal("Title");
            var authorIdIndex = reader.GetOrdinal("AuthorId");
            var libraryIdIndex = reader.GetOrdinal("LibraryId");
            var authorNameIndex = reader.GetOrdinal("AuthorName");

            while (reader.Read())
            {
                var book = new Book
                {
                    Id = reader.GetInt32(bookIdIndex),
                    Title = reader.GetString(titleIndex),
                    AuthorId = reader.GetInt32(authorIdIndex),
                    LibraryId = reader.GetInt32(libraryIdIndex),
                    Author = reader.IsDBNull(authorNameIndex) ? null : reader.GetString(authorNameIndex)
                };

                books.Add(book);
            }

            return books;
        }

        public Book GetBookFromDb(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var query = @"
        SELECT b.Id, b.Title, b.AuthorId, b.LibraryId, a.Name AS AuthorName
        FROM books b
        LEFT JOIN authors a ON b.AuthorId = a.Id
        WHERE b.Id = @id";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();

            var idIndex = reader.GetOrdinal("Id");
            var titleIndex = reader.GetOrdinal("Title");
            var authorIdIndex = reader.GetOrdinal("AuthorId");
            var libraryIdIndex = reader.GetOrdinal("LibraryId");
            var authorNameIndex = reader.GetOrdinal("AuthorName");

            if (reader.Read())
            {
                return new Book
                {
                    Id = reader.GetInt32(idIndex),
                    Title = reader.GetString(titleIndex),
                    AuthorId = reader.GetInt32(authorIdIndex),
                    LibraryId = reader.GetInt32(libraryIdIndex),
                    Author = !reader.IsDBNull(authorNameIndex) ? reader.GetString(authorNameIndex) : null
                };
            }

            return null;
        }

        public Book AddBookToDb(Book book)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var insertCommand = new NpgsqlCommand(
                "INSERT INTO books (title, authorid, libraryid) VALUES (@title, @aid, @lid) RETURNING id", connection);

            insertCommand.Parameters.AddWithValue("@title", book.Title);
            insertCommand.Parameters.AddWithValue("@aid", book.AuthorId);
            insertCommand.Parameters.AddWithValue("@lid", book.LibraryId);

            var id = (int)insertCommand.ExecuteScalar();
            book.Id = id;

            
            using var authorCommand = new NpgsqlCommand("SELECT name FROM authors WHERE id = @id", connection);
            authorCommand.Parameters.AddWithValue("@id", book.AuthorId);

            var authorNameObj = authorCommand.ExecuteScalar();
            book.Author = authorNameObj != null ? authorNameObj.ToString() : null;

            return book;
        }


        public void UpdateBookInDb(int id, Book book)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(
                "UPDATE books SET title = @title, authorid = @aid, libraryid = @lid WHERE id = @id",
                connection);

            command.Parameters.AddWithValue("@title", book.Title);
            command.Parameters.AddWithValue("@aid", book.AuthorId);
            command.Parameters.AddWithValue("@lid", book.LibraryId);
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }

        public void DeleteBookFromDb(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand("DELETE FROM books WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }

        public List<Book> GetBooksByAuthorId(int authorId)
        {
            var books = new List<Book>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

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

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@authorId", authorId);

            using var reader = command.ExecuteReader();

            // Dobijamo indekse kolona koristeći alias-e
            var bookIdIndex = reader.GetOrdinal("BookId");
            var titleIndex = reader.GetOrdinal("Title");
            var authorIdIndex = reader.GetOrdinal("AuthorId");
            var libraryIdIndex = reader.GetOrdinal("LibraryId");
            var authorNameIndex = reader.GetOrdinal("AuthorName");

            while (reader.Read())
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
        public List<Book> GetBooksByLibraryId(int libraryId)
        {
            var books = new List<Book>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand("SELECT * FROM books WHERE libraryid = @lid", connection);
            command.Parameters.AddWithValue("@lid", libraryId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                books.Add(new Book
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    AuthorId = reader.GetInt32(2),
                    Author = reader.GetString(3),
                    LibraryId = reader.GetInt32(4)
                });
            }

            return books;
        }

        public List<Genre> GetGenresByBookId(int bookId)
        {
            var genres = new List<Genre>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(
                @"SELECT g.id, g.name 
                  FROM genres g 
                  INNER JOIN book_genres bg ON g.id = bg.genreid 
                  WHERE bg.bookid = @bid", connection);

            command.Parameters.AddWithValue("@bid", bookId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                genres.Add(new Genre
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }

            return genres;
        }
    }
}
