using CrudService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrudService.Service
{
    public interface IBookService
    {
        Task<List<BookDto>> GetAllAsync();
        Task<BookDto?> GetByIdAsync(Guid id);
        Task<BookDto> CreateAsync(BookDto dto);
        Task<BookDto?> UpdateAsync(Guid id, BookDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}