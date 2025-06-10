

namespace BootcampApp.Models
{


    public class Book
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public int AuthorId {  get; set; }
        public int? LibraryId { get; set; }
        public List<Genre>? Genres { get; set; }
    }
}
