using MyShop.Data.Infrastructure;
using MyShop.Model.Models;

namespace MyShop.Data.Respositories
{
    public interface IFeedbackRepository : IRepository<Feedback>
    {

    }


    public class FeedbackRepository : RepositoryBase<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}