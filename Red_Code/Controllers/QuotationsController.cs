using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Red_Code.Application.DTOs;
using Red_Code.Application.Interfaces;

namespace Red_Code.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuotationsController : ControllerBase
    {
        private readonly IQuotationService _quotationService;

        public QuotationsController(IQuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuotationDto>>> GetAll()
        {
            var quotations = await _quotationService.GetAllQuotationsAsync();
            return Ok(quotations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuotationDto>> GetById(int id)
        {
            var quotation = await _quotationService.GetQuotationByIdAsync(id);
            if (quotation == null)
                return NotFound();

            return Ok(quotation);
        }

        [HttpPost]
        public async Task<ActionResult<QuotationDto>> Create([FromBody] CreateQuotationDto createQuotationDto)
        {
            var quotation = await _quotationService.CreateQuotationAsync(createQuotationDto);
            return CreatedAtAction(nameof(GetById), new { id = quotation.Id }, quotation);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<QuotationDto>> Update(int id, [FromBody] CreateQuotationDto updateQuotationDto)
        {
            var quotation = await _quotationService.UpdateQuotationAsync(id, updateQuotationDto);
            if (quotation == null)
                return NotFound();

            return Ok(quotation);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _quotationService.DeleteQuotationAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
