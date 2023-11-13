using BooKStore.Models.DTO.ViewModel;
using BooKStore.Models;
using Microsoft.AspNetCore.Mvc;
using BooKStore.Models.Domain;
using BooKStore.Models.DTO;

namespace BooKStore.Controllers
{
    public class CartController : Controller
    {
        private readonly DataBaseContext _context;

        public CartController(DataBaseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            CartViewModel cartVM = new()
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.Quantity * x.Price)
            };

            return View(cartVM);
        }

        public async Task<IActionResult> Add(int id)
        {
            Book book = await _context.Book.FindAsync(id);

            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            CartItem cartItem = cart.Where(c => c.BookId == id).FirstOrDefault();

            if (cartItem == null)
            {
                cart.Add(new CartItem(book));
            }
            else
            {
                cartItem.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", cart);

            TempData["msg"] = "Book has been added!";

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Decrease(long id)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

            CartItem cartItem = cart.Where(c => c.BookId == id).FirstOrDefault();

            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(p => p.BookId == id);
            }

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["msg"] = "Book has been removed!";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(long id)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

            cart.RemoveAll(p => p.BookId == id);

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["msg"] = "Book has been removed!";

            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Index");
        }

        //public IActionResult SmallCart()
        //{
        //    List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
        //    //HttpContext.Session.SetJson("Cart", cart);
        //    return View(cart);
        //}
    }
}
