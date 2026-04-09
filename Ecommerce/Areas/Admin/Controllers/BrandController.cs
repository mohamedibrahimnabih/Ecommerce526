using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class BrandController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IRepository<Brand> _repository;

        public BrandController(IRepository<Brand> repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index(int page = 1, string? query = null, CancellationToken cancellationToken = default)
        {
            //var brands = _context.Brands.AsQueryable();
            var brands = await _repository.GetAsync(cancellationToken: cancellationToken);

            // Filter
            if (query is not null)
            {
                brands = brands.Where(e => e.Name.ToLower().Contains(query.Trim().ToLower()));
                ViewBag.Query = query;
                //ViewData["Query"] = query;
            }

            // Pagination
            double totalPages = Math.Ceiling(brands.Count() / 3.0); 
            brands = brands.Skip((page - 1) * 3).Take(3);

            return View(new BrandsVM()
            {
                Brands = brands.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page,
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Brand());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Brand brand, IFormFile Img, CancellationToken cancellationToken = default)
        {
            if (Img is not null && Img.Length > 0)
            {
                var fileName = await CreateFileAsync(Img);
                brand.Logo = fileName;
            }

            await _repository.CreateAsync(brand, cancellationToken: cancellationToken);
            await _repository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Add Brand Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {
            //var brand = _context.Brands.Find(id);
            var brand = await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (brand is null) 
                return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Brand brand, IFormFile Img, CancellationToken cancellationToken = default)
        {
            //var brandInDB = _context.Brands.AsNoTracking().SingleOrDefault(e => e.Id == brand.Id);
            var brandInDB = await _repository.GetOneAsync(e => e.Id == brand.Id, tracked: false, cancellationToken: cancellationToken);

            if (brandInDB is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            if (Img is not null && Img.Length > 0)
            {
                // Create new img in wwwroot
                var fileName = await CreateFileAsync(Img);

                // Delete old img from wwwroot
                var oldFilePath = GetOldFilePath(brandInDB.Logo);
                if (oldFilePath is not null && System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);

                // Update img prop. in Db
                brand.Logo = fileName;
            }
            else
                brand.Logo = brandInDB.Logo;

            _repository.Update(brand);
            await _repository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Update Brand Successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleStatus(int id, CancellationToken cancellationToken = default)
        {
            //_context.Brands.Update(brand);

            //var brandInDB = _context.Brands.Find(id);
            var brandInDB = await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (brandInDB is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);
            brandInDB.Status = !brandInDB.Status;

            //_context.SaveChanges();
            await _repository.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            //var brand = _context.Brands.Find(id);
            var brand = await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (brand is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            // Delete old img from wwwroot
            var oldFilePath = GetOldFilePath(brand.Logo);
            if (oldFilePath is not null && System.IO.File.Exists(oldFilePath))
                System.IO.File.Delete(oldFilePath);

            _repository.Delete(brand);
            //_context.SaveChanges();
            await _repository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Delete Brand Successfully";

            return RedirectToAction(nameof(Index));
        }

        private async Task<string> CreateFileAsync(IFormFile Img)
        {
            var fileName =
                    $"{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}-{Guid.NewGuid().ToString()}{Path.GetExtension(Img.FileName)}";
            // 31290-fjkdsfhsd-32131.png

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brands", fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await Img.CopyToAsync(stream);
            }

            return fileName;
        }

        private string GetOldFilePath(string oldFileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brands", oldFileName);
            return filePath;
        }
    }
}
