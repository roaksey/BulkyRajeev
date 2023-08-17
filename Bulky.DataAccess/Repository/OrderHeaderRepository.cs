using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IReposiotry;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly AppDbContext _db;
        public OrderHeaderRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }      

        public void Update(OrderHeader obj)
        {
            _db.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderDb = _db.OrderHeaders.FirstOrDefault(x=>x.Id == id);
            if (orderDb != null)
            {
                orderDb.OrderStatus = orderStatus;
                if(!string.IsNullOrEmpty(paymentStatus)) orderDb.PaymentStatus = paymentStatus;
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string stripePaymentId)
        {
            throw new NotImplementedException();
        }
    }
}
