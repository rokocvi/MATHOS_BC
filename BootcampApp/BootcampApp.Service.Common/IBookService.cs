using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BootcampApp.Models;

namespace BootcampApp.Service.Common
{
    public interface IBookService
    {
        Task<List<Book>> GetAllBooksAsync(string? filter, string? sort, string? dir, int? page, int? size);
        Task<Book> GetBookAsync(int id);
        Task<Book> AddBookAsync(Book book);
        Task UpdateBookAsync(int id, Book book);
        Task DeleteBookAsync(int id);
        Task<List<Book>> GetBooksByAuthorAsync(int authorId,  string sortBy, string sortDirection, double? minRating);
        Task<List<Book>> GetBooksByLibraryAsync(int libraryId);
        Task<List<Genre>> GetGenresByBookAsync(int bookId, string? nameFilter, string? sortBy, string? sortDirection, int? page, int? pageSize);
        Task AddGenresToBookAsync(int bookId, IEnumerable<int> genreIds);
        Task <List<Loan>> GetLoansByUserIdAsync(int userId);
    }
}
