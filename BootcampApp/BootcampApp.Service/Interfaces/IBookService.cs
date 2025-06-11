using BootcampApp.Models;
using static BootcampApp.Models.BookController;

namespace BootcampApp.Service.Interfaces
{
    public interface IBookService
    {
        Task<List<Book>> GetAllBooksAsync();
        Task<Book> GetBookAsync(int id);
        Task<Book> AddBookAsync(Book book);
        void UpdateBook(int id, Book book);
        void DeleteBook(int id);
        List<Book> GetBooksByAuthor(int authorId);
        List<Book> GetBooksByLibrary(int libraryId);
        List<Genre> GetGenresByBook(int bookId);
    }
}
