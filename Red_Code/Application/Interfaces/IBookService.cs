using Red_Code.Application.DTOs;

namespace Red_Code.Application.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<BookDto> CreateBookAsync(CreateBookDto createBookDto);
        Task<BookDto?> UpdateBookAsync(int id, CreateBookDto updateBookDto);
        Task<bool> DeleteBookAsync(int id);
    }
}
