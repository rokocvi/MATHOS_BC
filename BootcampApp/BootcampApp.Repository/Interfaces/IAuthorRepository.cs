using BootcampApp.Models;
using System.Collections.Generic;

namespace BootcampApp.Repository
{
    public interface IAuthorRepository
    {
        List<Author> GetAll();
        Author GetById(int id);
        Author Create(Author author);
        bool Update(int id, Author author);
        bool Delete(int id);
        List<Book> GetBooksByAuthor(int authorId);
    }
}
