using Eshop.Extension;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Controllers.Components
{
    public class HeaderCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<Cart>>("GioHang");
            return View(cart);

        }
    }
}
