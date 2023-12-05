using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkTwice_Context;
using BLL;

namespace Presentation.DTO
{
    internal class TransactionDTO
    {
        private readonly CategoryRepository _categoryRepository = new CategoryRepository();

        public TransactionDTO(Transaction t)
        {
            Id = t.Id;
            UserId = t.UserId;
            Amount = t.Amount;
            Date = t.Date;
            Details = t.Details;
            Planned = t.Planned;
            FromCategory = _categoryRepository.GetCategoryById(t.FromCategory).Title;
            ToCategory = _categoryRepository.GetCategoryById(t.ToCategory).Title;
        }
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public String? FromCategory { get; set; }

        public String? ToCategory { get; set; }

        public decimal Amount { get; set; }

        public DateTime? Date { get; set; }

        public string? Details { get; set; }

        public bool Planned { get; set; }
    }
}
