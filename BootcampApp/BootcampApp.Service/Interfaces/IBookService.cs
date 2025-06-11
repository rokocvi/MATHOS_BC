using BootcampApp.Models;
using static BootcampApp.Models.BookController;

namespace BootcampApp.Service.Interfaces
{
    public interface IBookService
    {
        Task<List<Book>> GetAllBooksAsync(string? filter, string? sort, string? dir, int? page, int? size);
        Task<Book> GetBookAsync(int id);
        Task<Book> AddBookAsync(Book book);
        Task UpdateBookAsync(int id, Book book);
        Task DeleteBookAsync(int id);
        Task<List<Book>> GetBooksByAuthorAsync(int authorId);
        Task<List<Book>> GetBooksByLibraryAsync(int libraryId);
        Task<List<Genre>> GetGenresByBookAsync(int bookId);
    }
}
