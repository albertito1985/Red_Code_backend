using Microsoft.EntityFrameworkCore;
using Red_Code.Application.Interfaces;
using Red_Code.Domain.Entities;
using Red_Code.Infrastructure.Data;

namespace Red_Code.Infrastructure.Repositories
{
    public class QuotationRepository : IQuotationRepository
    {
        private readonly ApplicationDbContext _context;

        public QuotationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Quotation>> GetAllAsync()
        {
            return await _context.Quotations.ToListAsync();
        }

        public async Task<Quotation?> GetByIdAsync(int id)
        {
            return await _context.Quotations.FindAsync(id);
        }

        public async Task<Quotation> CreateAsync(Quotation quotation)
        {
            _context.Quotations.Add(quotation);
            await _context.SaveChangesAsync();
            return quotation;
        }

        public async Task<Quotation?> UpdateAsync(Quotation quotation)
        {
            var existingQuotation = await _context.Quotations.FindAsync(quotation.Id);
            if (existingQuotation == null)
                return null;

            existingQuotation.QuotationText = quotation.QuotationText;
            existingQuotation.Author = quotation.Author;

            await _context.SaveChangesAsync();
            return existingQuotation;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var quotation = await _context.Quotations.FindAsync(id);
            if (quotation == null)
                return false;

            _context.Quotations.Remove(quotation);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
