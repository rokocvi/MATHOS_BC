namespace WebAPI.Controllers
{
    public partial class BookController
    {
        public class AuthorWithBooks
        {
            public int AuthorId { get; set; }
            public string? AuthorName { get; set; }
            public int Age { get; set; }
            public List<Book> Books { get; set; } = new List<Book>();
        }

    }
}
