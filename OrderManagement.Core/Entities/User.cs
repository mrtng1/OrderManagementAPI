using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Core.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; }
}
