using AspNetCoreHero.ToastNotification.Abstractions;
using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly EshopContext _context;
        public INotyfService _notyfService { get; }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, EshopContext context, INotyfService notyfService)
        {
            _logger = logger;
            _context = context;
            _notyfService = notyfService;
        }

        public IActionResult Index()
        {
            //tong dt
            ViewBag.TongDT = TongDT();


            return View();
        }

        public int TongDT()
        {
            int tong = _context.InvoiceDetails.Sum(n => n.Quantity * n.UnitPrice);
            return tong;
        }

        #region DSHDMoiNhatTrongTuan
        public IActionResult DSHD(int? page)
        {
            DateTime weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);

            var latesInvoice = from m in _context.Invoices.Include(i => i.Account).Where(i => i.IssuedDate >= weekStart && i.Status == false)
                                                .OrderByDescending(i => i.IssuedDate)
                               select m;

            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;
            PagedList<Invoice> models = new PagedList<Invoice>(latesInvoice, pageNo, pageSize);

            ViewBag.CurrentPage = pageNo;

            return View(models);
        }
        #endregion

        #region DoanhThuTheoThoiGian
        public IActionResult DoanhThuTheoThoiGian(int time)
        {
            DateTime date = DateTime.Today;
            switch (time)
            {
                case 0:
                    {
                        var DT = _context.InvoiceDetails.Include(p => p.Invoice)
                                                        .Include(p => p.Product)
                                                        .Where(p => p.Invoice.IssuedDate.Month == date.Month)
                                                        .GroupBy(p => p.Invoice.IssuedDate.Day).Select(p => new Invoice { Id = p.Key, Total = p.Sum(p => p.UnitPrice - p.Product.Price * p.Quantity) })
                                                        .ToList();
                        ViewBag.DT = DT;
                        if (DT != null)
                            return View(DT);
                    }
                    break;
                case 1:
                    {
                        var DT = _context.InvoiceDetails.Include(p => p.Invoice)
                                                        .Include(p => p.Product)
                                                        .Where(p => p.Invoice.IssuedDate.Year == date.Year)
                                                        .GroupBy(p => p.Invoice.IssuedDate.Month).Select(p => new Invoice { Id = p.Key, Total = p.Sum(p => p.UnitPrice - p.Product.Price * p.Quantity) })
                                                        .ToList();
                        ViewBag.DT = DT;
                        if (DT != null)
                            return View(DT);
                    }
                    break;
                case 2:
                    {
                        var DT = _context.InvoiceDetails.Include(p => p.Invoice)
                                                        .Include(p => p.Product)
                                                        .GroupBy(p => p.Invoice.IssuedDate.Year).Select(p => new Invoice { Id = p.Key, Total = p.Sum(p => p.UnitPrice - p.Product.Price * p.Quantity) })
                                                        .ToList();
                        ViewBag.DT = DT;
                        if (DT != null)
                            return View(DT);
                    }
                    break;
                default: break;

            }
            return NotFound();
        }
        #endregion

        #region TopSanPhamBanChay
        //public IActionResult SPTop(int top)
        //{
        //    ViewBag.TopSP = top;
        //    var spTop = _context.InvoiceDetails.Include(p => p.Product)
        //                                       .GroupBy(i => i.Product.Name)
        //                                       .Select(a => new InvoiceDetail { ProductName = a.Key, Quantity = a.Sum(c => c.Quantity) })
        //                                       .OrderByDescending(i => i.Quantity)
        //                                       .ToList().Take(top);
        //    if (topProduct != null)
        //        return View(topProduct);

        //    return NotFound();
        //}
        #endregion

        #region TienChiCuaKH
        public IActionResult TienChiCuaKH()
        {
            var TienChi = _context.Invoices.Include(i => i.Account).GroupBy(i => i.Account.FullName)
                                           .Select(a => new Invoice { Name = a.Key, Total = a.Sum(b => b.Total) })
                                           .ToList();

            if (TienChi != null)
                return View(TienChi);

            return NotFound();
        }
        #endregion

        #region DangXuat
        [HttpGet]
        public IActionResult Logout()
        {
            var accountId = HttpContext.Session.GetString("AccountId");
            var _User = _context.Accounts.Where(x => x.Id == Convert.ToInt32(accountId)).FirstOrDefault();
            try
            {
                if (_User != null)
                {
                    _context.Update(_User);
                    _context.SaveChanges();
                }

                HttpContext.SignOutAsync();
                HttpContext.Session.Remove("AccountId");
                //add database Cart

                //xoa sesstion

                _notyfService.Warning("Đăng xuất thành công!");
                return RedirectToAction("Index", "Home", new { Area = "default" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}