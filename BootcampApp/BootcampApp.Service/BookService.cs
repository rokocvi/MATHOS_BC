using BootcampApp.Models;
using static BootcampApp.Models.BookController;
using BootcampApp.Repository.Common;
using BootcampApp.Service.Common;


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

        public async Task<List<Book>> GetBooksByAuthorAsync(int authorId, string sortBy, string sortDirection, double? minRating)
        {
            return await _bookRepository.GetBooksByAuthorIdAsync(authorId, sortBy, sortDirection, minRating);
        }

        public async Task<List<Book>> GetBooksByLibraryAsync(int libraryId)
        {
            return await _bookRepository.GetBooksByLibraryIdAsync(libraryId);
        }

        public async Task<List<Genre>> GetGenresByBookAsync(int bookId,string? nameFilter, string? sortBy ,  string? sortDirection,  int? page, int? pageSize)
        {
            return await _bookRepository.GetGenresByBookIdAsync(bookId, nameFilter, sortBy, sortDirection, page, pageSize);
        }

        public async Task AddGenresToBookAsync(int bookId, IEnumerable<int> genreIds)
        {
            await _bookRepository.AddGenresToBookAsync(bookId, genreIds);
        }
    }
}
