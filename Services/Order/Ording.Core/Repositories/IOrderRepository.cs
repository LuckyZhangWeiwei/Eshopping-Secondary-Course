using Ording.Core.Entities;

namespace Ording.Core.Repositories;

public interface IOrderRepository : IAsyncRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByUserName(string userName);
}
