namespace Red_Code.Domain.Entities
{
    public class Quotation
    {
        public int Id { get; set; }
        public string QuotationText { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
    }
}
