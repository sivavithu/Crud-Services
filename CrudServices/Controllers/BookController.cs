using CrudService.Models;
using CrudService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CrudService.Controllers
{
    [Route("")]
    [ApiController]
    [Authorize]  // Requires valid JWT for all endpoints
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("books")]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await _bookService.GetAllAsync());
        }

        [HttpGet("books/{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpPost("books")]
        public async Task<ActionResult> Create(BookDto dto)
        {
            var book = await _bookService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        [HttpPut("books/{id}")]
        public async Task<ActionResult> Update(Guid id, BookDto dto)
        {
            var book = await _bookService.UpdateAsync(id, dto);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpDelete("books/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var success = await _bookService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}