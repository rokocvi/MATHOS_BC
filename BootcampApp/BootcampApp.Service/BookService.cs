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

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllBooksFromDbAsync();
        }

        public async  Task<Book> GetBookAsync(int id)
        {
            return await _bookRepository.GetBookFromDbAsync(id);
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            return await _bookRepository.AddBookToDbAsync(book);
        }

        public void UpdateBook(int id, Book book)
        {
            _bookRepository.UpdateBookInDb(id, book);
        }

        public void DeleteBook(int id)
        {
            _bookRepository.DeleteBookFromDb(id);
        }

        public List<Book> GetBooksByAuthor(int authorId)
        {
            return _bookRepository.GetBooksByAuthorId(authorId);
        }

        public List<Book> GetBooksByLibrary(int libraryId)
        {
            return _bookRepository.GetBooksByLibraryId(libraryId);
        }

        public List<Genre> GetGenresByBook(int bookId)
        {
            return _bookRepository.GetGenresByBookId(bookId);
        }
    }
}
