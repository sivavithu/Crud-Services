using ClosedXML.Excel;
using CrudService.Models;
using CrudService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Threading.Tasks;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using Microsoft.AspNetCore.Hosting; // Use IWebHostEnvironment
using Microsoft.AspNetCore.Http; // For IFormFile

namespace CrudService.Controllers
{
    [Route("")]
    [ApiController]
    [Authorize] // Requires valid JWT for all endpoints
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IWebHostEnvironment _environment;

        public BookController(IBookService bookService, IWebHostEnvironment environment)
        {
            _bookService = bookService;
            _environment = environment;
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var success = await _bookService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost("books/upload-excel")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UploadExcel([FromForm] IFormFile file)
        {
            // Check for an invalid or empty file and ensure it's an .xlsx file.
            if (file == null || file.Length == 0 || !file.FileName.EndsWith(".xlsx"))
            {
                return BadRequest("Invalid Excel file");
            }

            var webRootPath = _environment.WebRootPath;
            if (string.IsNullOrEmpty(webRootPath))
            {
                return StatusCode(500, "WebRootPath is not configured. Ensure a 'wwwroot' folder exists.");
            }

            var uploadsPath = Path.Combine(webRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Create a unique filename to avoid conflicts.
            var filePath = Path.Combine(uploadsPath, Guid.NewGuid() + Path.GetExtension(file.FileName));

            try
            {
                // Save the file to the server.
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Read the Excel file using ClosedXML.
                using var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheet(1); // Assume the first worksheet.
                var lastRowUsed = worksheet.LastRowUsed();
                var rowCount = lastRowUsed?.RowNumber() ?? 0;

                if (rowCount < 2)
                {
                    // Clean up the temporary file if it's empty/invalid before returning.
                    System.IO.File.Delete(filePath);
                    return BadRequest("Excel file is empty or lacks data rows.");
                }

                // Validate the headers in the first row. This is a crucial check.
                if (worksheet.Cell(1, 1).GetString() != "Name" || worksheet.Cell(1, 2).GetString() != "Author")
                {
                    // Clean up the temporary file if headers are invalid.
                    System.IO.File.Delete(filePath);
                    return BadRequest("Excel file must have 'Name' and 'Author' headers in the first row.");
                }

                int booksAdded = 0;
                int duplicatesSkipped = 0;

                for (int row = 2; row <= rowCount; row++)
                {
                    var name = worksheet.Cell(row, 1).GetString()?.Trim() ?? string.Empty;
                    var author = worksheet.Cell(row, 2).GetString()?.Trim() ?? string.Empty;

                    // Skip any rows that don't have both a name and an author.
                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(author))
                    {
                        continue;
                    }

                    var dto = new BookDto { Name = name, Author = author };

                    // This assumes you have an ExistsAsync method in your service.
                    // if (await _bookService.ExistsAsync(dto.Name, dto.Author))
                    // {
                    //     duplicatesSkipped++;
                    //     continue; // Skip the duplicate book.
                    // }

                    await _bookService.CreateAsync(dto);
                    booksAdded++;
                }

                // Clean up the temporary file after successful processing.
                System.IO.File.Delete(filePath);

                return Ok($"Excel file processed successfully. {booksAdded} books added, {duplicatesSkipped} duplicates skipped.");
            }
            catch (Exception ex)
            {
                // Always try to delete the file in case of an error during processing.
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                Console.WriteLine($"Error in UploadExcel: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, $"Error importing Excel: {ex.Message}");
            }
        }

        [HttpPost("books/generate-pdf")]
        [Produces("application/pdf")]
        public IActionResult GeneratePdf([FromBody] GeneratePdfRequest request)
        {
            if (request == null || request.Books == null || request.Books.Count == 0)
            {
                return BadRequest("No books provided in the request body.");
            }

            using (var stream = new MemoryStream())
            {
                using (var pdf = new PdfDocument())
                {
                    var page = pdf.AddPage();
                    var gfx = XGraphics.FromPdfPage(page);
                    var font = new XFont("Montserrat", 12, XFontStyleEx.Regular);
                    var titleFont = new XFont("Montserrat", 16, XFontStyleEx.Bold);

                    gfx.DrawString("My Library Books", titleFont, XBrushes.Black, new XRect(20, 20, page.Width, 50), XStringFormats.TopLeft);

                    int y = 70;
                    gfx.DrawString("Title", font, XBrushes.Black, new XRect(20, y, 300, 20), XStringFormats.TopLeft);
                    gfx.DrawString("Author", font, XBrushes.Black, new XRect(320, y, 200, 20), XStringFormats.TopLeft);
                    y += 20;

                    foreach (var book in request.Books)
                    {
                        gfx.DrawString(book.Name, font, XBrushes.Black, new XRect(20, y, 300, 20), XStringFormats.TopLeft);
                        gfx.DrawString(book.Author, font, XBrushes.Black, new XRect(320, y, 200, 20), XStringFormats.TopLeft);
                        y += 20;
                    }

                    pdf.Save(stream);
                }

                return File(stream.ToArray(), "application/pdf", "books.pdf");
            }
        }
    }
}
