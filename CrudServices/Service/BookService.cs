using CrudService.Data;
using CrudService.Entities;
using CrudService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrudService.Service
{
    public class BookService : IBookService
    {
        private readonly BookDbContext _context;

        public BookService(BookDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookDto>> GetAllAsync()
        {
            var books = await _context.Books.ToListAsync();
            return books.Select(b => new BookDto { Id = b.Id, Name = b.Name, Author = b.Author }).ToList();
        }

        public async Task<BookDto?> GetByIdAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;
            return new BookDto { Id = book.Id, Name = book.Name, Author = book.Author };
        }

        public async Task<BookDto> CreateAsync(BookDto dto)
        {
            var book = new Book { Id = Guid.NewGuid(), Name = dto.Name, Author = dto.Author };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return new BookDto { Id = book.Id, Name = book.Name, Author = book.Author };
        }

        public async Task<BookDto?> UpdateAsync(Guid id, BookDto dto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;
            book.Name = dto.Name;
            book.Author = dto.Author;
            await _context.SaveChangesAsync();
            return new BookDto { Id = book.Id, Name = book.Name, Author = book.Author };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}