using BootcampApp.Models;
using static BootcampApp.Models.BookController;

namespace BootcampApp.Repository.Interfaces
{
    public interface IBookRepository
    {
        List<Book> GetAllBooksFromDb();
        Book GetBookFromDb(int id);
        Book AddBookToDb(Book book);
        void UpdateBookInDb(int id, Book book);
        void DeleteBookFromDb(int id);
        List<Book> GetBooksByAuthorId(int authorId);
        List<Book> GetBooksByLibraryId(int libraryId);
        List<Genre> GetGenresByBookId(int bookId);
    }
}
