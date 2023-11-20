using System;
using System.Collections.Generic;

namespace ThinkTwice_Context
{
    public partial class Transaction
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? FromCategory { get; set; }

        public Guid? ToCategory { get; set; }

        public decimal Amount { get; set; }

        public DateTime? Date { get; set; }

        public string Title { get; set; } = null!;

        public string? Details { get; set; }

        public bool Planned { get; set; }

        public virtual Category? FromCategoryNavigation { get; set; }

        public virtual Category? ToCategoryNavigation { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
