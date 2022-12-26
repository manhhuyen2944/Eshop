using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InvoicesController : Controller
    {
        private readonly EshopContext _context;
        public InvoicesController(EshopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region DSHDChuaDuyet
        public IActionResult DSHDChuaDuyet(string sortOrder, string searchStr, int? page)
        {
            var eshopDH = from m in _context.Invoices.Include(o => o.Account).Where(o => o.Status == false).OrderByDescending(x => x.IssuedDate) select m;

            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                eshopDH = eshopDH.Where(p => p.Id.ToString().Contains(searchStr) || p.AccountId.ToString().Contains(searchStr));
            }

            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            ViewBag.CurrentPage = pageNo;
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "FullName");
            PagedList<Invoice> models = new PagedList<Invoice>(eshopDH, pageNo, pageSize);
            return View(models);
        }
        #endregion

        #region DSHDDaDuyet
        public IActionResult DSHDDaDuyet(string searchStr, int? page)
        {

            var paidProduct = from m in _context.Invoices.Include(o => o.Account).Where(o => o.Status == true).OrderByDescending(x => x.IssuedDate) select m;

            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                paidProduct = paidProduct.Where(p => p.Id.ToString().Contains(searchStr) || p.AccountId.ToString().Contains(searchStr));
            }


            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 15;
            ViewBag.CurrentPage = pageNo;
            ViewData["CustomerId"] = new SelectList(_context.Accounts, "Id", "FullName");
            PagedList<Invoice> models = new PagedList<Invoice>(paidProduct, pageNo, pageSize);
            return View(models);
        }
        #endregion

        #region Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(o => o.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            var ctdh = _context.InvoiceDetails
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.InvoiceId == invoice.Id)
                .OrderBy(x => x.Id)
                .ToList();
            ViewBag.ChiTiet = ctdh;
            return View(invoice);
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewBag.CustomerId = new SelectList(_context.Accounts, "Id", "FullName", invoice.AccountId);
            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,AccountId,IssuedDate,ShippingAddress,ShippingPhone,Total,Status")] Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return NotFound();
            }

            try
            {
                var donhang = _context.Invoices.AsNoTracking().Include(x => x.Account).Where(x => x.Id == id).FirstOrDefault();
                if (donhang != null)
                {
                    donhang.Status = true;
                    donhang.IssuedDate = invoice.IssuedDate;

                    var invoicedetail = _context.InvoiceDetails
                                                .AsNoTracking()
                                                .Where(x => x.Id == donhang.Id)
                                                .Include(x => x.Product)
                                                .ToList();

                    if (donhang.Status == true)
                    {
                        string chitiet = "";
                        for (int i = 0; i < invoicedetail.Count(); i++)
                        {
                            var product = _context.Products
                                .Where(x => x.Id == invoicedetail[i].ProductId)
                                .FirstOrDefault();
                            chitiet += "Tên sản phẩm : " + product.Name.ToString();
                            chitiet += " - Số lượng : " + invoicedetail[i].Quantity.ToString();
                            chitiet += " - Giá : " + invoicedetail[i].Product.Price.ToString("#,##0") + " VNĐ";
                            chitiet += " - Thành tiền : " + (invoicedetail[i].Quantity * invoicedetail[i].UnitPrice).ToString("#,##0") + " VNĐ";
                            chitiet += "<br />";
                            chitiet += "Tổng thanh toán: " + donhang.Total.ToString("#,##0") + " VNĐ";
                            product.Stock = (product.Stock - invoicedetail[i].Quantity);

                            _context.Update(product);
                        }
                    }
                }
                _context.Update(donhang);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DonHangExists(invoice.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            ViewBag.CustomerId = new SelectList(_context.Accounts, "Id", "FullName", invoice.AccountId);
            return RedirectToAction("DSHDChuaDuyet", "Invoices");
        }
        #endregion

        private bool DonHangExists(int id)
        {
            return _context.Invoices.Any(e => e.Id == id);
        }
    }
}
