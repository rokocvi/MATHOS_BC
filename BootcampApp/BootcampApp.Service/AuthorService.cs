using BootcampApp.Models;
using BootcampApp.Repository;
using System.Collections.Generic;

namespace BootcampApp.Service
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _repository;

        public AuthorService(IAuthorRepository repository)
        {
            _repository = repository;
        }

        public List<Author> GetAllAuthors() => _repository.GetAll();
        public Author GetAuthorById(int id) => _repository.GetById(id);
        public Author CreateAuthor(Author author) => _repository.Create(author);
        public bool UpdateAuthor(int id, Author author) => _repository.Update(id, author);
        public bool DeleteAuthor(int id) => _repository.Delete(id);
        public List<Book> GetBooksByAuthor(int authorId) => _repository.GetBooksByAuthor(authorId);
    }
}
