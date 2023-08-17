using Bulky.Models;

namespace Bulky.DataAccess.Repository.IReposiotry
{
    public interface IOrderDetailRepository:IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);
    }
}