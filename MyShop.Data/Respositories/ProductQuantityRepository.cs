using MyShop.Data.Infrastructure;
using MyShop.Model.Models;

namespace MyShop.Data.Respositories
{
    public interface IProductQuantityRepository : IRepository<ProductQuantity>
    {
    }

    public class ProductQuantityRepository : RepositoryBase<ProductQuantity>, IProductQuantityRepository
    {
        public ProductQuantityRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}