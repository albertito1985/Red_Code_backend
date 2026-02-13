using Red_Code.Application.DTOs;
using Red_Code.Application.Interfaces;
using Red_Code.Domain.Entities;

namespace Red_Code.Application.Services
{
    public class QuotationService : IQuotationService
    {
        private readonly IQuotationRepository _quotationRepository;

        public QuotationService(IQuotationRepository quotationRepository)
        {
            _quotationRepository = quotationRepository;
        }

        public async Task<IEnumerable<QuotationDto>> GetAllQuotationsAsync()
        {
            var quotations = await _quotationRepository.GetAllAsync();
            return quotations.Select(quotation => new QuotationDto
            {
                Id = quotation.Id,
                Quotation = quotation.QuotationText,
                Author = quotation.Author
            });
        }

        public async Task<QuotationDto?> GetQuotationByIdAsync(int id)
        {
            var quotation = await _quotationRepository.GetByIdAsync(id);
            if (quotation == null)
                return null;

            return new QuotationDto
            {
                Id = quotation.Id,
                Quotation = quotation.QuotationText,
                Author = quotation.Author
            };
        }

        public async Task<QuotationDto> CreateQuotationAsync(CreateQuotationDto createQuotationDto)
        {
            var quotation = new Quotation
            {
                QuotationText = createQuotationDto.Quotation,
                Author = createQuotationDto.Author
            };

            var createdQuotation = await _quotationRepository.CreateAsync(quotation);

            return new QuotationDto
            {
                Id = createdQuotation.Id,
                Quotation = createdQuotation.QuotationText,
                Author = createdQuotation.Author
            };
        }

        public async Task<QuotationDto?> UpdateQuotationAsync(int id, CreateQuotationDto updateQuotationDto)
        {
            var existingQuotation = await _quotationRepository.GetByIdAsync(id);
            if (existingQuotation == null)
                return null;

            existingQuotation.QuotationText = updateQuotationDto.Quotation;
            existingQuotation.Author = updateQuotationDto.Author;

            var updatedQuotation = await _quotationRepository.UpdateAsync(existingQuotation);
            if (updatedQuotation == null)
                return null;

            return new QuotationDto
            {
                Id = updatedQuotation.Id,
                Quotation = updatedQuotation.QuotationText,
                Author = updatedQuotation.Author
            };
        }

        public async Task<bool> DeleteQuotationAsync(int id)
        {
            return await _quotationRepository.DeleteAsync(id);
        }
    }
}
