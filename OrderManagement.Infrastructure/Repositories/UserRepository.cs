using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.Infrastructure.Repositories;

public class UserRepository : IRepository<User>
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext context)
    {
        _db = context;
    }
    
    public User? Get(Guid id)
    {
        return _db.Users
            .FirstOrDefault(o => o.Id == id);
    }

    public List<User> GetAll()
    {
        return _db.Users
            .ToList();
    }
    

    public void Add(User user)
    {
        _db.Users.Add(user);
        _db.SaveChanges();
    }

    public void Edit(User user)
    {
        _db.Users.Update(user);
        _db.SaveChanges();
    }
}