using BootcampApp.Models;
using System.Collections.Generic;

namespace BootcampApp.Service
{
    public interface IAuthorService
    {
        List<Author> GetAllAuthors();
        Author GetAuthorById(int id);
        Author CreateAuthor(Author author);
        bool UpdateAuthor(int id, Author author);
        bool DeleteAuthor(int id);
        List<Book> GetBooksByAuthor(int authorId);
    }
}
