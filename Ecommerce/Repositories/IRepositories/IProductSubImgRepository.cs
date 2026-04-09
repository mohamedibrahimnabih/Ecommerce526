namespace Ecommerce.Repositories.IRepositories
{
    public interface IProductSubImgRepository : IRepository<ProductSubImg>
    {
        void DeleteRange(IEnumerable<ProductSubImg> productSubImgs);
    }
}
