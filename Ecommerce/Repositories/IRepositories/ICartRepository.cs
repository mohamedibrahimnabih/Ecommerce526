namespace Ecommerce.Repositories.IRepositories
{
    public interface ICartRepository : IRepository<Cart>
    {
        void DeleteRange(IEnumerable<Cart> carts);
    }
}
