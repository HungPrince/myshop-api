using MyShop.Data.Infrastructure;
using MyShop.Model.Models;

namespace MyShop.Data.Respositories
{
    public interface IColorRepository : IRepository<Color>
    {
    }

    public class ColorRepository : RepositoryBase<Color>, IColorRepository
    {
        public ColorRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}