using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.Infrastructure.Repositories;

public class UserRepository : IRepository<User>
{
    private readonly AppDbContext db;

    public UserRepository(AppDbContext context)
    {
        db = context;
    }
    
    public User? Get(Guid id)
    {
        return db.Users
            .FirstOrDefault(o => o.Id == id);
    }

    public List<User> GetAll()
    {
        return db.Users
            .ToList();
    }
    

    public void Add(User user)
    {
        db.Users.Add(user);
        db.SaveChanges();
    }

    public void Edit(User user)
    {
        db.Users.Update(user);
        db.SaveChanges();
    }
}