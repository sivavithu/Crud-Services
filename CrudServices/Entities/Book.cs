using System;

namespace CrudService.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
    }
}