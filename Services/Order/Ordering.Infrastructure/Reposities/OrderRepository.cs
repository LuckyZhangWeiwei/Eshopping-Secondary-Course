using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Reposities;
using Ording.Core.Entities;
using Ording.Core.Repositories;

public class OrderRepository : RepositoryBase<Order>, IOrderRepository
{
    public OrderRepository(OrderContext dbContext)
        : base(dbContext) { }

    public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
    {
        var orderList = await _dbContext.Orders.Where(o => o.UserName == userName).ToListAsync();
        return orderList;
    }
}
