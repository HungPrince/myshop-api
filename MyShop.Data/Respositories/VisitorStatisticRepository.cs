using MyShop.Data.Infrastructure;
using MyShop.Model.Models;


namespace MyShop.Data.Respositories
{
    public interface IVisitorStatisticRepository : IRespository<VisitorStatistic>
    {
    }

    public class VisitorStatisticRepository : RespositoryBase<VisitorStatistic>, IVisitorStatisticRepository
    {
        public VisitorStatisticRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
