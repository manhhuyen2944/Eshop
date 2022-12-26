using AspNetCoreHero.ToastNotification.Abstractions;
using Eshop.Data;
using Eshop.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly EshopContext _context;
        public INotyfService _notyfService { get; }

        public InvoicesController(EshopContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [Route("chi-tiet-don-hang-{id}.html", Name = "Chitietdonhang")]
        public IActionResult Details(int? id)
        {
            ViewBag.lsTypes = _context.ProductTypes.ToList();
            if (id == null)
            {
                return NotFound();
            }
            var taikhoannID = HttpContext.Session.GetString("AccountId");

            if (string.IsNullOrEmpty(taikhoannID))
                return RedirectToAction("Login", "Accounts");

            var khachhang = _context.Accounts.SingleOrDefault(x => x.Id == Convert.ToInt32(taikhoannID));

            if (khachhang == null)
                return NotFound();

            var donhang = _context.Invoices
                .Include(x => x.Account)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id && Convert.ToInt32(taikhoannID) == x.AccountId);

            if (donhang == null)
            {
                return NotFound();
            }

            var chitietdonhang = _context.InvoiceDetails
                .Include(x => x.Product).AsNoTracking()
                .Where(x => x.InvoiceId == id)
                .OrderBy(x => x.Id).ToList();

            InvoiceViewModel donHang = new InvoiceViewModel();
            donHang.Invoices = donhang;
            donHang.InvoiceDetails = chitietdonhang;

            ViewBag.donHang = donHang;
            return View(donHang);
        }



    }
}
