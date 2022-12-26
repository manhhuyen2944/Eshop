using AspNetCoreHero.ToastNotification.Abstractions;
using Eshop.Data;
using Eshop.Extension;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Eshop.Controllers
{
    public class CartsController : Controller
    {
        private readonly EshopContext _context;
        public INotyfService _notyfService { get; }
        public CartsController(EshopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        public IActionResult Index()
        {
            ViewBag.lsTypes = _context.ProductTypes.ToList();
            ViewBag.AccountId = HttpContext.Session.GetString("AccountId");
            return View(GioHang);
        }

        public List<Cart> GioHang
        {
            get
            {
                var cart = HttpContext.Session.Get<List<Cart>>("GioHang");
                if (cart == default(List<Cart>))
                {
                    cart = new List<Cart>();
                    HttpContext.Session.Set("GioHang", cart);
                }
                return cart;
            }
        }

        private int Exists(List<Cart> carts, int id)
        {
            for (var i = 0; i < carts.Count; i++)
            {
                if (carts[i].Product.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }


        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productId, int? qty)
        {
            List<Cart> carts = GioHang;
            try
            {
                Product sp = _context.Products.SingleOrDefault(p => p.Id == productId);

                Cart item = GioHang.SingleOrDefault(x => x.Product.Id == productId);

                if (carts == null)
                {
                    carts.Add(new Cart
                    {
                        Quantity = qty.HasValue ? qty.Value : 1,
                        Product = sp
                    });
                }
                else
                {
                    int index = Exists(carts, productId);
                    if (index == -1)
                    {
                        carts.Add(new Cart
                        {
                            Quantity = qty.HasValue ? qty.Value : 1,
                            Product = sp
                        });
                    }
                    else
                    {
                        carts[index].Quantity += qty.Value;
                    }
                }

                GetSession.Set(HttpContext.Session, "GioHang", carts);
                _notyfService.Success("Thêm thành công vào giỏ hàng");
                return Json(new { succcess = true });
            }
            catch
            {
                return Json(new { succcess = false });
            }
        }

        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productId, int? qty)
        {
            List<Cart> carts = GioHang;
            try
            {
                if (carts != null)
                {
                    Product sp = _context.Products.SingleOrDefault(p => p.Id == productId);
                    int index = Exists(carts, productId);
                    if (index == -1)
                    {
                        carts.Add(new Cart
                        {
                            Quantity = qty.HasValue ? qty.Value : 1,
                            Product = sp
                        });
                    }
                    else
                    {
                        carts[index].Quantity = qty.Value;
                    }

                    GetSession.Set(HttpContext.Session, "GioHang", carts);
                    _notyfService.Success("Update thành công");
                    return Json(new { succcess = true });
                }
                return Json(new { succcess = false });
            }
            catch
            {
                return Json(new { succcess = false });
            }
        }

        [HttpPost]
        [Route("api/cart/remove")]
        public IActionResult Remove(int productId)
        {
            try
            {
                List<Cart> carts = GioHang;
                Cart item = carts.SingleOrDefault(a => a.Product.Id == productId);
                if (item != null)
                {
                    carts.Remove(item);
                }
                GetSession.Set(HttpContext.Session, "GioHang", carts);
                _notyfService.Success("Xóa thành công sản phẩm khỏi giỏ hàng");
                return Json(new { succcess = true });
            }
            catch (Exception)
            {
                return Json(new { succcess = false });
            }
        }

        public IActionResult RemoveAll(int? productId)
        {
            List<Cart> carts = GioHang;
            Cart item = carts.SingleOrDefault(a => a.Product.Id == productId);
            if (item != null)
            {
                carts.RemoveAll(u => u.ProductId == productId);
                HttpContext.Session.Set("GioHang", carts);
            }
            else
            {
                HttpContext.Session.Remove("GioHang");
            }
            return RedirectToAction("Index");


        }

        //[HttpPost]
        //[Route("api/cart/remove")]
        //public IActionResult RemoveAll(int productId)
        //{
        //    try
        //    {
        //        List<Cart> carts = GioHang;
        //        Cart item = carts.SingleOrDefault(a => a.Product.Id == productId);
        //        if (item != null)
        //        {
        //            carts.RemoveAll(item);
        //        }
        //        GetSession.Set(HttpContext.Session, "GioHang", carts);
        //        _notyfService.Success("Xóa thành công giỏ hàng");
        //        return Json(new { succcess = true });
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { succcess = false });
        //    }
        //}

    }
}
