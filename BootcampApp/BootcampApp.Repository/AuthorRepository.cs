using BootcampApp.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BootcampApp.Repository
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly string _connectionString;

        public AuthorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private NpgsqlConnection GetConnection() => new NpgsqlConnection(_connectionString);

        public async Task<List<Author>> GetAllAsync()
        {
            var authors = new List<Author>();
            await using var conn = GetConnection();
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand("SELECT id, name FROM authors", conn);
            await using var reader = cmd.ExecuteReader();

            while (await reader.ReadAsync())
            {
                authors.Add(new Author
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }
            return authors;
        }

        public async Task<Author> GetByIdAsync(int id)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand("SELECT id, name FROM authors WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);
            await using var reader = cmd.ExecuteReader();
            if (await reader.ReadAsync())
            {
                return new Author
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };
            }
            return null;
        }

        public async Task<Author> CreateAsync(Author author)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand("INSERT INTO authors (name) VALUES (@name) RETURNING id", conn);
            cmd.Parameters.AddWithValue("name", author.Name);
            author.Id = (int)cmd.ExecuteScalar();
            return author;
        }

        public async Task<bool> UpdateAsync(int id, Author author)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand("UPDATE authors SET name = @name WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("name", author.Name);
            cmd.Parameters.AddWithValue("id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var conn = GetConnection();
            conn.Open();

            // Provjera postoji li povezanih knjiga
            using var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM books WHERE authorid = @id", conn);
            checkCmd.Parameters.AddWithValue("id", id);
            var count = (long)checkCmd.ExecuteScalar();
            if (count > 0)
                return false;

            using var cmd = new NpgsqlCommand("DELETE FROM authors WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public List<Book> GetBooksByAuthor(int authorId)
        {
            var books = new List<Book>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT id, title, authorid FROM books WHERE authorid = @aid", conn);
            cmd.Parameters.AddWithValue("aid", authorId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                books.Add(new Book
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    AuthorId = reader.GetInt32(2)
                });
            }
            return books;
        }
    }
}
