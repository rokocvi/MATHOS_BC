using BootcampApp.Models;
using BootcampApp.Repository;
using System.Collections.Generic;
using BootcampApp.Service.Common;
using BootcampApp.Repository.Common;

namespace BootcampApp.Service
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _repository;

        public AuthorService(IAuthorRepository repository)
        {
            _repository = repository;
        }

        public  Task<List<Author>> GetAllAuthors() => _repository.GetAllAsync();
        public Task<Author> GetAuthorByIdAsync(int id) => _repository.GetByIdAsync(id);
        public Task<Author> CreateAuthorAsync(Author author) => _repository.CreateAsync(author);
        public Task<bool> UpdateAuthorAsync(int id, Author author) => _repository.UpdateAsync(id, author);
        public bool DeleteAuthor(int id) => _repository.Delete(id);
        public List<Book> GetBooksByAuthor(int authorId) => _repository.GetBooksByAuthor(authorId);
    }
}
