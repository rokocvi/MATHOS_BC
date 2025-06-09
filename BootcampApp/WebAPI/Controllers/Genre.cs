namespace WebAPI.Controllers
{
    public partial class BookController
    {
        public class Genre
        {
            public int Id { get; set; }
            public string? Name { get; set; }

            // Navigacija prema knjigama (opcionalno)
            public List<Book>? Books { get; set; };
        }


    }
}
