using Red_Code.Domain.Entities;

namespace Red_Code.Application.Interfaces
{
    public interface IQuotationRepository
    {
        Task<IEnumerable<Quotation>> GetAllAsync();
        Task<Quotation?> GetByIdAsync(int id);
        Task<Quotation> CreateAsync(Quotation quotation);
        Task<Quotation?> UpdateAsync(Quotation quotation);
        Task<bool> DeleteAsync(int id);
    }
}
