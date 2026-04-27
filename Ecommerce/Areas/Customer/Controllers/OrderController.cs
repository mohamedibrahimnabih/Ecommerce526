using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace Ecommerce.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Order> _orderRepository;

        public OrderController(UserManager<ApplicationUser> userManager, IRepository<Order> orderRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> Index(string? query = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var userOrders = await _orderRepository.GetAsync(e => e.ApplicationUserId == user.Id);

            if(query is not null)
                userOrders = userOrders.Where(e => e.Id == Convert.ToInt32(query));

            return View(userOrders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepository.GetOneAsync(e=>e.Id == id);

            return View(order);
        }

        public async Task<IActionResult> Refund(int id)
        {
            var order = await _orderRepository.GetOneAsync(e => e.Id == id);

            if (order is null) return NotFound();

            if (order.PaymentStatus == PaymentStatus.Refunded || order.OrderStatus == OrderStatus.Canceled)
                return BadRequest();

            var options = new RefundCreateOptions()
            {
                Reason = RefundReasons.Unknown,
                Amount = ((long)order.TotalPrice * 100) - (5 * 100),
                PaymentIntent = order.TransactionId
            };

            var service = new RefundService();
            var session = service.Create(options);

            order.OrderStatus = OrderStatus.Canceled;
            order.PaymentStatus = PaymentStatus.Refunded;
            await _orderRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
