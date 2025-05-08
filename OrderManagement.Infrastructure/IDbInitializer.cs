namespace OrderManagement.Infrastructure;

public interface IDbInitializer
{
    void Initialize(AppDbContext context);
}