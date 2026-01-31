using EduVN.Domain.Entities;

namespace EduVN.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByOrderCodeAsync(long orderCode);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
}
