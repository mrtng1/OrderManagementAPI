using OrderManagement.Core.Entities;

namespace OrderManagement.Core.Interfaces;

public interface IUserService
{
    List<User> GetAllUsers();
    User CreateUser(string username);
    User GetUser(Guid id);
}