using Red_Code.Application.DTOs;

namespace Red_Code.Application.Interfaces
{
    public interface IQuotationService
    {
        Task<IEnumerable<QuotationDto>> GetAllQuotationsAsync();
        Task<QuotationDto?> GetQuotationByIdAsync(int id);
        Task<QuotationDto> CreateQuotationAsync(CreateQuotationDto createQuotationDto);
        Task<QuotationDto?> UpdateQuotationAsync(int id, CreateQuotationDto updateQuotationDto);
        Task<bool> DeleteQuotationAsync(int id);
    }
}
