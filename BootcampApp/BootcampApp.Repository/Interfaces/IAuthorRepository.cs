using BootcampApp.Models;
using System.Collections.Generic;

namespace BootcampApp.Repository
{
    public interface IAuthorRepository
    {
        Task<List<Author>> GetAllAsync();
        Task<Author> GetByIdAsync(int id);
        Task<Author> CreateAsync(Author author);
        Task<bool> UpdateAsync(int id, Author author);
        bool Delete(int id);
        List<Book> GetBooksByAuthor(int authorId);
    }
}
