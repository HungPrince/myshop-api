using MyShop.Data.Infrastructure;
using MyShop.Model.Models;

namespace MyShop.Data.Respositories
{
    public interface IAnnouncementUserRepository : IRespository<AnnouncementUser>
    {
    }

    public class AnnouncementUserRepository : RespositoryBase<AnnouncementUser>, IAnnouncementUserRepository
    {
        public AnnouncementUserRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}