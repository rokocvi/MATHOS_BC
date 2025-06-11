using BootcampApp.Models;
using System.Collections.Generic;

namespace BootcampApp.Service
{
    public interface IAuthorService
    {
        Task<List<Author>> GetAllAuthors();
        Task<Author> GetAuthorByIdAsync(int id);
        Task<Author> CreateAuthorAsync(Author author);
        Task<bool> UpdateAuthorAsync(int id, Author author);
        bool DeleteAuthor(int id);
        List<Book> GetBooksByAuthor(int authorId);
    }
}
