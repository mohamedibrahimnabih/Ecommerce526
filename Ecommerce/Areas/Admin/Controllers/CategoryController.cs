using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IRepository<Category> _repository;// = new();

        public CategoryController(IRepository<Category> repository) // new Repository<Category>()
        {
            //_context = new();
            _repository = repository;
            // = new Repository<Category>();
        }

        public async Task<IActionResult> Index(int page = 1, string? query = null, CancellationToken cancellationToken = default)
        {
            //var categories = _context.Categories.AsQueryable();
            var categories = await _repository.GetAsync(cancellationToken: cancellationToken);

            // Filter
            if (query is not null)
            {
                categories = categories.Where(e => e.Name.ToLower().Contains(query.Trim().ToLower()));
                ViewBag.Query = query;
                //ViewData["Query"] = query;
            }

            // Pagination
            double totalPages = Math.Ceiling(categories.Count() / 3.0); 
            categories = categories.Skip((page - 1) * 3).Take(3);

            return View(new CategoriesVM()
            {
                Categories = categories.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page,
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category, CancellationToken cancellationToken = default)
        {
            if(!ModelState.IsValid)
                return View(category);

            //_context.Categories.Add(category);
            //_context.SaveChanges();

            await _repository.CreateAsync(category, cancellationToken);
            await _repository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Add Category Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {
            //var category = _context.Categories.Find(id);
            var category = await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
             
            if (category is null) 
                return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Category category, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return View(category);

            //_context.Categories.Update(category);
            _repository.Update(category);

            //var categoryInDB = _context.Categories.Find(category.Id);
            //if (categoryInDB is null)
            //    return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);
            //categoryInDB.Name = category.Name;
            //categoryInDB.Status = category.Status;

            await _repository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Update Category Successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleStatus(int id, CancellationToken cancellationToken = default)
        {
            //_context.Categories.Update(category);

            //var categoryInDB = _context.Categories.Find(id);
            var categoryInDB = await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (categoryInDB is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);
            categoryInDB.Status = !categoryInDB.Status;

            //_context.SaveChanges();
            await _repository.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            //var category = _context.Categories.Find(id);
            var category = await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (category is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            _repository.Delete(category);
            //_context.Categories.Remove(category);
            //_context.SaveChanges();
            await _repository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Delete Category Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
