using AspNetCoreHero.ToastNotification.Abstractions;
using Eshop.Data;
using Eshop.Extension;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using System.Xml.Linq;

namespace Eshop.Controllers
{
    public class CheckoutsController : Controller
    {
        private readonly EshopContext _context;
        public INotyfService _notyfService { get; }
        public CheckoutsController(EshopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
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

        public IActionResult Index(string url = null)
        {
            ViewBag.lsTypes = _context.ProductTypes.ToList();
            List<Cart> carts = GioHang;

            var _accountId = HttpContext.Session.GetString("AccountId");

            Invoice model = new Invoice();

            if (_accountId != null)
            {
                var _account = _context.Accounts.SingleOrDefault(x => x.Id == Convert.ToInt32(_accountId));

                model.Code = DateTime.Now.ToString("yyyyMMddhhmmss");
                model.AccountId = _account.Id;
                model.IssuedDate = DateTime.Now;
                model.ShippingPhone = _account.Phone;
                model.ShippingAddress = _account.Address;
                model.Status = false;
                model.Total = Convert.ToInt32(carts.Sum(x => x.Product.Price * x.Quantity));

            }
            ViewBag.GioHang = carts;
            return View(model);
        }

        [HttpPost]
        public IActionResult Index([Bind("Id,Code,AccountId,ShippingAddress,ShippingPhone,Total,Status")] Invoice invoices)
        {

            List<Cart> carts = GioHang;

            var _accountId = HttpContext.Session.GetString("AccountId");

            if (_accountId != null)
            {
                var _account = _context.Accounts.SingleOrDefault(x => x.Id == Convert.ToInt32(_accountId));

                Invoice invoice = new Invoice()
                {
                    Code = DateTime.Now.ToString("yyyyMMddhhmmss"),
                    AccountId = _account.Id,
                    IssuedDate = DateTime.Now,
                    ShippingPhone = invoices.ShippingPhone,
                    ShippingAddress = invoices.ShippingAddress,
                    Total = Convert.ToInt32(carts.Sum(x => x.Product.Price * x.Quantity)),
                    Status = false

                };

                _context.Update(_account);
                _context.SaveChanges();
            }


            try
            {
                Invoice donhang = new Invoice
                {
                    Code = DateTime.Now.ToString("yyyyMMddhhmmss"),
                    AccountId = invoices.AccountId,
                    ShippingAddress = invoices.ShippingAddress,
                    ShippingPhone = invoices.ShippingPhone,
                    IssuedDate = DateTime.Now,
                    Status = false,
                    Total = Convert.ToInt32(carts.Sum(x => x.Product.Price * x.Quantity))
                };
                _context.Add(donhang);
                _context.SaveChanges();


                foreach (var item in carts)
                {
                    InvoiceDetail invoiceDetail = new InvoiceDetail()
                    {
                        InvoiceId = donhang.Id,
                        ProductId = item.Product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                    };

                    _context.Add(invoiceDetail);
                }

                _context.SaveChanges();
                HttpContext.Session.Remove("GioHang");
                _notyfService.Success("Đơn đặt thành công. Đang chờ xét duyệt");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                throw;
            }
        }
        
    }
}
