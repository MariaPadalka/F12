namespace BLL.DTO
{
    using ThinkTwice_Context;

    public class UserDTO
    {
        public UserDTO(User user)
        {
            this.Id = user.Id;
            this.Email = user.Email;
            this.Name = user.Name;
            this.Surname = user.Surname;
            this.BirthDate = user.BirthDate;
            this.Currency = user.Currency;
            this.Categories = user.Categories;
            this.Transactions = user.Transactions;
        }

        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public DateTime? BirthDate { get; set; }

        public string Currency { get; set; } = null!;

        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
