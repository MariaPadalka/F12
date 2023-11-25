using System;
using System.Collections.Generic;

namespace ThinkTwice_Context
{
    public partial class User
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public DateTime? BirthDate { get; set; }

        public string Currency { get; set; } = null!;

        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
