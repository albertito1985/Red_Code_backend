using Red_Code.Application.DTOs;
using Red_Code.Application.Interfaces;
using Red_Code.Domain.Entities;

namespace Red_Code.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return books.Select(book => new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author
            });
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
                return null;

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author
            };
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
        {
            var book = new Book
            {
                Title = createBookDto.Title,
                Author = createBookDto.Author
            };

            var createdBook = await _bookRepository.CreateAsync(book);

            return new BookDto
            {
                Id = createdBook.Id,
                Title = createdBook.Title,
                Author = createdBook.Author
            };
        }

        public async Task<BookDto?> UpdateBookAsync(int id, CreateBookDto updateBookDto)
        {
            var existingBook = await _bookRepository.GetByIdAsync(id);
            if (existingBook == null)
                return null;

            existingBook.Title = updateBookDto.Title;
            existingBook.Author = updateBookDto.Author;

            var updatedBook = await _bookRepository.UpdateAsync(existingBook);
            if (updatedBook == null)
                return null;

            return new BookDto
            {
                Id = updatedBook.Id,
                Title = updatedBook.Title,
                Author = updatedBook.Author
            };
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            return await _bookRepository.DeleteAsync(id);
        }
    }
}
