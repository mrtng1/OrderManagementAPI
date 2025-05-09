using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;

namespace OrderManagement.UnitTests.Fakes;

public class FakeUserRepository : IRepository<User>
{
    private readonly List<User> _users = new();

    public bool AddWasCalled { get; private set; } = false;
    public bool EditWasCalled { get; private set; } = false;
    public bool RemoveWasCalled { get; private set; } = false;

    public FakeUserRepository(List<User>? seedUsers = null)
    {
        if (seedUsers != null)
        {
            _users = new List<User>(seedUsers);
        }
    }

    public User? Get(Guid id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public List<User> GetAll()
    {
        return _users.ToList();
    }

    public void Add(User user)
    {
        AddWasCalled = true;
        _users.Add(user);
    }

    public void Edit(User user)
    {
        EditWasCalled = true;
        var index = _users.FindIndex(u => u.Id == user.Id);
        if (index != -1)
        {
            _users[index] = user;
        }
    }

    public void Remove(Guid id)
    {
        RemoveWasCalled = true;
        _users.RemoveAll(u => u.Id == id);
    }
}