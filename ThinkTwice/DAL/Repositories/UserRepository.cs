namespace ThinkTwice_Context
{
    using Microsoft.EntityFrameworkCore;

    public class UserRepository
    {
        private readonly ThinkTwiceContext context = new ThinkTwiceContext();

        public virtual User? GetUserById(Guid userId)
        {
            return this.context.Users.FirstOrDefault(u => u.Id == userId);
        }

        public virtual User? GetUserByEmail(string email)
        {
            return this.context.Users.FirstOrDefault(u => u.Email == email);
        }

        public void Add(User user)
        {
            this.context.Users.Add(user);
            this.context.SaveChanges();
        }

        public void Update(User user)
        {
            this.context.Entry(user).State = EntityState.Modified;
            this.context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var user = this.context.Users.Find(id);
            if (user != null)
            {
                this.context.Users.Remove(user);
                this.context.SaveChanges();
            }
        }
    }
}