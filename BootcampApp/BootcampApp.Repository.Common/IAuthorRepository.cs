using BootcampApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootcampApp.Repository.Common
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
