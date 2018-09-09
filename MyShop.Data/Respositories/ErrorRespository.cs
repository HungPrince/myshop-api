using MyShop.Data.Infrastructure;
using MyShop.Model.Models;

namespace MyShop.Data.Respositories
{
    public interface IErrorRespository : IRespository<Error>
    {
    }

    public class ErrorRespository : RespositoryBase<Error>, IErrorRespository
    {
        public ErrorRespository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}