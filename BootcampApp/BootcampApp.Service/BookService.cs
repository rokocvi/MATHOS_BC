using BootcampApp.Models;
using BootcampApp.Repository.Interfaces;
using BootcampApp.Service.Interfaces;
using static BootcampApp.Models.BookController;

namespace BootcampApp.Service
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<List<Book>> GetAllBooksAsync(string? filter, string? sort, string? dir, int? page, int? size)
        {
            return await _bookRepository.GetAllBooksFromDbAsync(filter, sort, dir, page, size);
        }

        public async  Task<Book> GetBookAsync(int id)
        {
            return await _bookRepository.GetBookFromDbAsync(id);
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            return await _bookRepository.AddBookToDbAsync(book);
        }

        public async Task UpdateBookAsync(int id, Book book)
        {
            await _bookRepository.UpdateBookInDbAsync(id, book);
        }

        public async Task DeleteBookAsync(int id)
        {
            await _bookRepository.DeleteBookFromDbAsync(id);
        }

        public async Task<List<Book>> GetBooksByAuthorAsync(int authorId)
        {
            return await _bookRepository.GetBooksByAuthorIdAsync(authorId);
        }

        public async Task<List<Book>> GetBooksByLibraryAsync(int libraryId)
        {
            return await _bookRepository.GetBooksByLibraryIdAsync(libraryId);
        }

        public async Task<List<Genre>> GetGenresByBookAsync(int bookId)
        {
            return await _bookRepository.GetGenresByBookIdAsync(bookId);
        }
    }
}
