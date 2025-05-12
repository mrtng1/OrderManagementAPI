namespace OrderManagement.Core.Interfaces;

public interface IRepository<T>
{
    List<T> GetAll();
    T? Get(Guid id);
    void Add(T entity);
    void Edit(T entity);
}