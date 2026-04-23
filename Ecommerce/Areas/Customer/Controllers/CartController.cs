using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace Ecommerce.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Promotion> _promotionRepository;

        public CartController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository, IRepository<Product> productRepository, IRepository<Promotion> promotionRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _promotionRepository = promotionRepository;
        }

        public async Task<IActionResult> Index(string? promotionCode = null, CancellationToken cancellationToken = default)
        
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var userCart = await _cartRepository
                .GetAsync(e => e.ApplicationUserId == user.Id, 
                includes: [e => e.Product],
                cancellationToken: cancellationToken);

            if(promotionCode is not null)
            {
                // Check if promotionCode is exist & is valid
                var promotion = await _promotionRepository.GetOneAsync(e => e.Code == promotionCode && e.Usage > 0);
                if (promotion is null)
                {
                    TempData["error-notification"] = "Invalid promotion";
                    return View(userCart);
                }

                // Check product list in cart, matching product list in promotion code
                if (promotion.ProductId is null)
                {
                    // Apply discount
                    var cartTotal = userCart.Sum(e => e.TotalPrice);
                    var discount = cartTotal - (userCart.Sum(e => e.TotalPrice) * promotion.Discount / 100);

                    //
                } 
                else
                {   
                    //userCart.Select(e => e.ProductId).ToList().Contains(promotion.ProductId);

                    foreach (var item in userCart)
                    {
                        if(item.ProductId == promotion.ProductId)
                        {
                            var cartTotal = item.TotalPrice;
                            var applyDiscount = cartTotal - (item.TotalPrice * promotion.Discount / 100);

                            item.PricePerProduct = applyDiscount;
                            item.TotalPrice = item.PricePerProduct * item.Count;
                            await _cartRepository.CommitAsync();

                            TempData["success-notification"] = "Apply Code Successfully";
                        }
                    }

                    if (promotion is null) TempData["error-notification"] = "Can not apply this promotion code on the product in the current list";
                }
            }

            return View(userCart);
        }

        public async Task<IActionResult> AddToCart(int productId, int count, CancellationToken cancellationToken)
        {
            // Retrieve User from cookies
            var user = await _userManager.GetUserAsync(User);
            if(user is null) return NotFound();

            var product = await _productRepository.GetOneAsync(e => e.Id == productId, cancellationToken: cancellationToken);
            if (product is null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == user.Id);

            if(cart is null)
            {
                await _cartRepository.CreateAsync(new()
                {
                    ApplicationUserId = user.Id,
                    ProductId = productId,
                    Count = count,
                    PricePerProduct = product.Price,
                    TotalPrice = product.Price * count
                }, cancellationToken: cancellationToken);
            }
            else
            {
                cart.Count += count;
                cart.PricePerProduct = product.Price;
                cart.TotalPrice = product.Price * count;
            }

            await _cartRepository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Add Product Successfully to the cart";

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        public async Task<IActionResult> IncrementCount(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == user.Id);

            if (cart is null) return NotFound();

            cart.Count += 1;
            await _cartRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DecrementCount(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == user.Id);

            if (cart is null) return NotFound();

            if(cart.Count > 1)
            {
                cart.Count -= 1;
                await _cartRepository.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == user.Id);

            if (cart is null) return NotFound();

            _cartRepository.Delete(cart);
            await _cartRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Pay()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/cancel",
            };

            var userCart = await _cartRepository
                .GetAsync(e => e.ApplicationUserId == user.Id,
                includes: [e => e.Product]);

            foreach (var item in userCart)
            {
                options.LineItems.Add(
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                                Description = item.Product.Description,
                            },
                            UnitAmount = (long)item.PricePerProduct * 100,
                        },
                        Quantity = item.Count,
                    });
            }


            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }
    }
}
