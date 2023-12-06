namespace Presentation.DTO
{
    using System;
    using BLL;
    using ThinkTwice_Context;

    internal class TransactionDTO
    {
        private readonly CategoryRepository categoryRepository = new CategoryRepository();

        public TransactionDTO(Transaction t)
        {
            this.Id = t.Id;
            this.UserId = t.UserId;
            this.Amount = t.Amount;
            this.Date = t.Date?.ToString("yyyy-MM-dd");
            this.Details = t.Details;
            this.Planned = t.Planned;
            this.FromCategory = this.categoryRepository.GetCategoryById(t.FromCategory).Title;
            this.ToCategory = this.categoryRepository.GetCategoryById(t.ToCategory).Title;
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string? FromCategory { get; set; }

        public string? ToCategory { get; set; }

        public decimal Amount { get; set; }

        public string? Date { get; set; }

        public string? Details { get; set; }

        public bool Planned { get; set; }
    }
}
