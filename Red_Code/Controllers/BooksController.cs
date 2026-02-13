using Microsoft.AspNetCore.Mvc;
using Red_Code.Application.DTOs;
using Red_Code.Application.Interfaces;

namespace Red_Code.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        // NO USE FOR THIS?

        //[HttpGet("{id}")]
        //public async Task<ActionResult<BookDto>> GetById(int id)
        //{
        //    var book = await _bookService.GetBookByIdAsync(id);
        //    if (book == null)
        //        return NotFound();

        //    return Ok(book);
        //}

        [HttpPost]
        public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto createBookDto)
        {
            var book = await _bookService.CreateBookAsync(createBookDto);
            return CreatedAtAction(nameof(GetAll), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BookDto>> Update(int id, [FromBody] CreateBookDto updateBookDto)
        {
            var book = await _bookService.UpdateBookAsync(id, updateBookDto);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
