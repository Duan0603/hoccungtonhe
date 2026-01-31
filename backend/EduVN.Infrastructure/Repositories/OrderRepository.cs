using EduVN.Application.Interfaces;
using EduVN.Domain.Entities;
using EduVN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduVN.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByOrderCodeAsync(long orderCode)
    {
        return await _context.Orders
            .Include(o => o.Student)
            .Include(o => o.Course)
            .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
    }

    public async Task AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}
