using System;

namespace CrudService.Models
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
    }

    public class GeneratePdfRequest
    {
        public List<BookDto> Books { get; set; }
    }
}