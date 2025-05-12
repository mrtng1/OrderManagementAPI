namespace OrderManagement.Core.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "Undefined";
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
