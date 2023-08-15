using Bulky.Models;

namespace Bulky.DataAccess.Repository.IReposiotry
{
    internal interface IOrderDetailRepository:IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);
    }
}