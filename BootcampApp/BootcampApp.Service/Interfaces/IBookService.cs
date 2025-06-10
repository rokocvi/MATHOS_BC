using BootcampApp.Models;
using static BootcampApp.Models.BookController;

namespace BootcampApp.Service.Interfaces
{
    public interface IBookService
    {
        List<Book> GetAllBooks();
        Book GetBook(int id);
        Book AddBook(Book book);
        void UpdateBook(int id, Book book);
        void DeleteBook(int id);
        List<Book> GetBooksByAuthor(int authorId);
        List<Book> GetBooksByLibrary(int libraryId);
        List<Genre> GetGenresByBook(int bookId);
    }
}
