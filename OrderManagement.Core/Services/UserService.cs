using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.Core.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepo;

    public UserService(IRepository<User> userRepo)
    {
        _userRepo = userRepo;
    }
    
    public List<User> GetAllUsers() => _userRepo.GetAll();
    public User GetUser(Guid id) => _userRepo.Get(id);
    
    public User CreateUser(string username)
    {
        User user = new User
        {
            Username = username
        };

        _userRepo.Add(user);
        return user;
    }
}