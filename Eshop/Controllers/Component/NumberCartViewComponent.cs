using Eshop.Extension;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Controllers.Component
{
    public class NumberCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            
            var cart = HttpContext.Session.Get<List<Cart>>("GioHang");
            int soluong = 0;
            if (cart != null)
            {
                soluong = cart.Count();
            }
            return View(cart);
        }
    }
}
