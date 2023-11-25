using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkTwice_Context;

namespace BLL.DTO
{
    public partial class UserDTO
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public DateTime? BirthDate { get; set; }

        public string Currency { get; set; } = null!;

        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public UserDTO(User user)
        {
            Id = user.Id;
            Email = user.Email;
            Name = user.Name;
            Surname = user.Surname;
            BirthDate = user.BirthDate;
            Currency = user.Currency;
            Categories = user.Categories;
            Transactions = user.Transactions;
        }
    }
}
