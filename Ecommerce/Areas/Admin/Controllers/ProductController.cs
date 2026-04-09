using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class ProductController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IRepository<Product> _repository;
        private readonly IProductSubImgRepository _productSubImgRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IProductService _productService;

        public ProductController(IRepository<Product> repository, IProductSubImgRepository productSubImgRepository, IRepository<Category> categoryRepository, IRepository<Brand> brandRepository, IProductService productService)
        {
            _repository = repository;
            _productSubImgRepository = productSubImgRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _productService = productService;
        }

        public async Task<IActionResult> Index(ProductFilterVM productFilterVM, int page = 1, CancellationToken cancellationToken = default)
        {
            //var products = _context.Products
                //.Include(e => e.Category)
                //.Include(e => e.Brand)
                //.AsQueryable();

            var products = await _repository.GetAsync(includes: [e => e.Category, e => e.Brand], cancellationToken: cancellationToken);

            // Filter
            if (productFilterVM.Name is not null)
            {
                products = products.Where(e => e.Name.ToLower().Contains(productFilterVM.Name.Trim().ToLower()));
                ViewBag.Name = productFilterVM.Name;
            }

            if(productFilterVM.MinPrice is not null)
            {
                products = products.Where(e => e.Price > productFilterVM.MinPrice);
                ViewBag.MinPrice = productFilterVM.MinPrice;
            }

            if (productFilterVM.MaxPrice is not null)
            {
                products = products.Where(e => e.Price < productFilterVM.MaxPrice);
                ViewBag.MaxPrice = productFilterVM.MaxPrice;
            }

            if(productFilterVM.CategoryId is not null)
            {
                products = products.Where(e => e.CategoryId == productFilterVM.CategoryId);
                ViewBag.CategoryId = productFilterVM.CategoryId;
            }

            if (productFilterVM.BrandId is not null)
            {
                products = products.Where(e => e.BrandId == productFilterVM.BrandId);
                ViewBag.BrandId = productFilterVM.BrandId;
            }

            // Pagination
            double totalPages = Math.Ceiling(products.Count() / 3.0); 
            products = products.Skip((page - 1) * 3).Take(3);

            return View(new ProductsVM()
            {
                Products = products.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page,
                Categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken),
                Brands = await _brandRepository.GetAsync(cancellationToken: cancellationToken)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
        {
            ViewBag.Categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.Brands = await _brandRepository.GetAsync(cancellationToken: cancellationToken);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile Img, List<IFormFile>? SubImgs, 
            CancellationToken cancellationToken = default)
        {
            if (Img is not null && Img.Length > 0)
            {
                var fileName = await _productService.CreateFileAsync(Img);
                product.MainImg = fileName;
            }

            //_context.Products.Add(product);
            await _repository.CreateAsync(product, cancellationToken: cancellationToken);
            await _repository.CommitAsync(cancellationToken: cancellationToken);

            if(SubImgs is not null)
            {
                foreach (var item in SubImgs)
                {
                    if (item is not null && item.Length > 0)
                    {
                        var fileName = await _productService.CreateFileAsync(item, ProductImgType.SubImg);
                        await _productSubImgRepository.CreateAsync(new()
                        {
                            ProductId = product.Id,
                            SubImg = fileName
                        }, cancellationToken: cancellationToken);
                    }
                }
                await _repository.CommitAsync(cancellationToken: cancellationToken);
            }

            //Response.Cookies.Append("success-notification", "Add Product Successfully");
            TempData["success-notification"] = "Add Product Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {
            //var product = _context.Products.Find(id);
            var product = await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (product is null) 
                return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            return View(new ProductWithSubImgsVM()
            {
                Product = product,
                ProductSubImgs = await _productSubImgRepository.GetAsync(e => e.ProductId == id, cancellationToken: cancellationToken),
                Categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken),
                Brands = await _brandRepository.GetAsync(cancellationToken: cancellationToken)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile Img, List<IFormFile>? SubImgs, CancellationToken cancellationToken = default)
        {
            //var productInDB = _context.Products.AsNoTracking().SingleOrDefault(e => e.Id == product.Id);
            var productInDB = await _repository.GetOneAsync(e => e.Id == product.Id, tracked: false, cancellationToken: cancellationToken);
            if (productInDB is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            if (Img is not null && Img.Length > 0)
            {
                // Create new img in wwwroot
                var fileName = await _productService.CreateFileAsync(Img);

                // Delete old img from wwwroot
                var oldFilePath = _productService.GetOldFilePath(productInDB.MainImg);
                if (oldFilePath is not null && System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);

                // Update img prop. in Db
                product.MainImg = fileName;
            }
            else
                product.MainImg = productInDB.MainImg;

            _repository.Update(product);
            await _repository.CommitAsync(cancellationToken: cancellationToken);

            if (SubImgs is not null)
            {
                //var productSubImgs = _context.productSubImgs.Where(e => e.ProductId == product.Id);
                var productSubImgs = await _productSubImgRepository.GetAsync(e => e.ProductId == product.Id, cancellationToken: cancellationToken);

                foreach (var item in productSubImgs)
                {
                    var oldFilePath = _productService.GetOldFilePath(item.SubImg, ProductImgType.SubImg);
                    if(System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                _productSubImgRepository.DeleteRange(productSubImgs);
                //_context.productSubImgs.RemoveRange(productSubImgs);

                foreach (var item in SubImgs)
                {
                    if (item is not null && item.Length > 0)
                    {
                        var fileName = await _productService.CreateFileAsync(item, ProductImgType.SubImg);
                        await _productSubImgRepository.CreateAsync(new()
                        {
                            ProductId = product.Id,
                            SubImg = fileName
                        }, cancellationToken: cancellationToken);
                    }
                }
                await _repository.CommitAsync(cancellationToken: cancellationToken);
            }

            TempData["success-notification"] = "Update Product Successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleStatus(int id, CancellationToken cancellationToken = default)
        {
            //_context.Products.Update(product);

            //var productInDB = _context.Products.Find(id);
            var productInDB = await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            
            if (productInDB is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);
            productInDB.Status = !productInDB.Status;

            await _repository.CommitAsync(cancellationToken: cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var product = await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (product is null)
                return RedirectToAction(nameof(HomeController.NotFoundPage), SD.HOME_CONTROLLER);

            // Delete old img from wwwroot
            var oldFilePath = _productService.GetOldFilePath(product.MainImg);
            if (oldFilePath is not null && System.IO.File.Exists(oldFilePath))
                System.IO.File.Delete(oldFilePath);

            _repository.Delete(product);
            await _repository.CommitAsync(cancellationToken: cancellationToken);

            TempData["success-notification"] = "Delete Product Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
