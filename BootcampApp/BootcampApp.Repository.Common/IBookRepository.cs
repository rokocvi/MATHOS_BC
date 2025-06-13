using BootcampApp.Models;

namespace BootcampApp.Repository.Common
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAllBooksFromDbAsync(string? titleFilter = null,
            string? sortBy = null,
            string? sortDirection = "asc",
            int? page = 1,
            int? pageSize = 10);
        Task<Book> GetBookFromDbAsync(int id);
        Task<Book> AddBookToDbAsync(Book book);
        Task UpdateBookInDbAsync(int id, Book book);
        Task DeleteBookFromDbAsync(int id);
        Task<List<Book>> GetBooksByAuthorIdAsync(int authorId, string sortBy = "Title", string sortDirection = "ASC", double? minRating = null);
        Task<List<Book>> GetBooksByLibraryIdAsync(int libraryId);
        Task<List<Genre>> GetGenresByBookIdAsync(int bookId, string? nameFilter = null, string? sortBy = null, string? sortDirection = "asc", int? page = null, int? pageSize = null);
        Task AddGenresToBookAsync(int bookId, IEnumerable<int> genreIds);
    }
}