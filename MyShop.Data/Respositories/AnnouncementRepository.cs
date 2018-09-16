using MyShop.Data.Infrastructure;
using MyShop.Model.Models;
using System.Data.Entity;
using System.Linq;

namespace MyShop.Data.Respositories
{
    public interface IAnnouncementRepository : IRepository<Announcement>
    {
        IQueryable<Announcement> GetAllUnread(string annoucementId);
    }

    public class AnnouncementRepository : RepositoryBase<Announcement>, IAnnouncementRepository
    {
        public AnnouncementRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public IQueryable<Announcement> GetAllUnread(string userId)
        {
            var query = (from x in DbContext.Announcements
                         join y in DbContext.AnnouncementUsers
                         on x.ID equals y.AnnouncementId
                         into xy
                         from y in xy.DefaultIfEmpty()
                         where (y.HasRead == false) && (y.UserId == null || y.UserId == userId)
                         select x).Include(x => x.AppUser);
            return query;
        }
    }
}