namespace BootcampApp.Models
{
    public partial class BookController
    {
        public class Library
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Address { get; set; }

            public List<Book>? Books { get; set; }
        }




    }
}
