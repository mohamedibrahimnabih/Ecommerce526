using Ecommerce.Repositories.IRepositories;

namespace Ecommerce.Repositories
{
    public class ProductSubImgRepository : Repository<ProductSubImg>, IProductSubImgRepository
    {
        public ProductSubImgRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void DeleteRange(IEnumerable<ProductSubImg> productSubImgs)
            => _context.RemoveRange(productSubImgs);
    }
}
