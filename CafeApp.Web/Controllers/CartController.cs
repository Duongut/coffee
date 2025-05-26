using CafeApp.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CafeApp.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private const string CartSessionKey = "ShoppingCart";

        public CartController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var cart = GetCartFromSession();
            var cartItems = new List<CartItemViewModel>();

            foreach (var item in cart)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                if (product != null)
                {
                    cartItems.Add(new CartItemViewModel
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        UnitPrice = product.Price,
                        Quantity = item.Quantity,
                        TotalPrice = product.Price * item.Quantity,
                        Notes = item.Notes,
                        Size = item.Size,
                        Temperature = item.Temperature
                    });
                }
            }

            ViewBag.CartTotal = cartItems.Sum(c => c.TotalPrice);
            return View(cartItems);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1, string notes = "", string size = "", string temperature = "")
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null || !product.IsAvailable)
            {
                return Json(new { success = false, message = "Product not found or unavailable" });
            }

            var cart = GetCartFromSession();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId && 
                                                       c.Size == size && 
                                                       c.Temperature == temperature);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                if (!string.IsNullOrEmpty(notes))
                {
                    existingItem.Notes = notes;
                }
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Notes = notes,
                    Size = size,
                    Temperature = temperature
                });
            }

            SaveCartToSession(cart);
            
            var cartCount = cart.Sum(c => c.Quantity);
            return Json(new { success = true, message = "Item added to cart", cartCount = cartCount });
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity, string size = "", string temperature = "")
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(c => c.ProductId == productId && 
                                               c.Size == size && 
                                               c.Temperature == temperature);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
                SaveCartToSession(cart);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/RemoveItem
        [HttpPost]
        public IActionResult RemoveItem(int productId, string size = "", string temperature = "")
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(c => c.ProductId == productId && 
                                               c.Size == size && 
                                               c.Temperature == temperature);

            if (item != null)
            {
                cart.Remove(item);
                SaveCartToSession(cart);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/Clear
        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction(nameof(Index));
        }

        // GET: Cart/GetCartCount
        public IActionResult GetCartCount()
        {
            var cart = GetCartFromSession();
            var count = cart.Sum(c => c.Quantity);
            return Json(new { count = count });
        }

        private List<CartItem> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return JsonConvert.DeserializeObject<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCartToSession(List<CartItem> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Temperature { get; set; } = string.Empty;
    }

    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Temperature { get; set; } = string.Empty;
    }
}
