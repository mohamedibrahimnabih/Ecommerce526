using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Ecommerce.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;// = new();

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? categoryName)
        {
            // Products
            var products = _context.Products.AsQueryable();

            // Include With Products
            products = products.Include(e => e.Category);

            // Categories
            // Solve 1
            //var categories = _context.Categories.ToList();

            //Dictionary<string, int> categoryWithTotal = new();

            //foreach (var item in categories)
            //{
            //    categoryWithTotal.Add(item.Name, products.Where(e => e.CategoryId == item.Id).Count());
            //}

            // Solve 2
            var categoryWithTotal = products
                .GroupBy(e => e.Category.Name)
                .Select(e => new
                {
                    e.Key,
                    count = e.Count()
                }).ToDictionary(e => e.Key, e => e.count);

            // Filter Products
            if(categoryName is not null)
                products = products.Where(e => e.Category.Name.Contains(categoryName));

            // Pagination Products
            products = products.Skip(0).Take(8);

            return View(new ProductsWithCategoriesVM()
            {
                Products = products.AsEnumerable(),
                CategoryWithTotal = categoryWithTotal
            });
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(e => e.Category)
                .SingleOrDefault(e => e.Id == id);

            if (product is null) return NotFound();

            product.Traffic += 1;
            _context.SaveChanges();

            var sameCategory = _context.Products
                .Where(e => e.CategoryId == product.CategoryId && e.Id != product.Id)
                .Include(e => e.Category)
                .Skip(0)
                .Take(4);

            var sameName = _context.Products
                .Where(e => e.Name.Contains(product.Name) && e.Id != product.Id)
                .Include(e => e.Category)
                .Skip(0)
                .Take(4);

            var topProducts = _context.Products
                .Where(e => e.Id != product.Id)
                .Include(e => e.Category)
                .OrderByDescending(e => e.Traffic)
                .Skip(0)
                .Take(4);

            // ToDo: Retrieve product with the same range price

            return View(new ProductWithRelatedVM()
            {
                Product = product,
                SameCategory = sameCategory.AsEnumerable(),
                SameName = sameName.AsEnumerable(),
                TopProducts = topProducts.AsEnumerable()
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Welcome()
        {
            return View();
        }






        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
