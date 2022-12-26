using AspNetCoreHero.ToastNotification.Abstractions;
using Eshop.Data;
using Eshop.Helpper;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Principal;
using X.PagedList;

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly EshopContext _context;
        public static string image;
        public INotyfService _notyfService { get; }

        public ProductsController(EshopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        public IActionResult Index(string sortOrder, string currentFilter, string searchStr, int? page)
        {
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "Name");
            var _product = from m in _context.Products.Include(p => p.ProductType) select m;

            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["UnitSortParm"] = sortOrder == "Unit" ? "unit_desc" : "Unit";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["CurrentSort"] = sortOrder;
            switch (sortOrder)
            {
                case "id_desc":
                    _product = _product.OrderBy(p => p.Id);
                    break;
                case "unit_desc":
                    _product = _product.OrderByDescending(p => p.Stock);
                    break;
                case "Unit":
                    _product = _product.OrderBy(p => p.Stock);
                    break;
                case "name_desc":
                    _product = _product.OrderByDescending(p => p.Name);
                    break;
                case "Name":
                    _product = _product.OrderBy(p => p.Name);
                    break;
                default:
                    _product = _product.OrderByDescending(p => p.Id);
                    break;
            }

            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                _product = _product.Where(p => p.Name.Contains(searchStr) || p.Id.ToString().Contains(searchStr) || p.Price.ToString().Contains(searchStr));
            }

            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 6;
            PagedList<Product> models = new PagedList<Product>(_product, pageNo, pageSize);
            ViewBag.CurrentPage = pageNo;

            return View(models);
        }

        #region Detail
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        #endregion

        #region Create
        public IActionResult Create()
        {
            ViewBag.ProductType = new SelectList(_context.ProductTypes.ToList(), "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SKU,Name,Description,ProductTypeId,Price,Stock,Image,Status")] Product product, IFormFile fImage)
        {

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[10];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            product.SKU = new String(stringChars);

            product.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(product.Name);

            if (fImage != null)
            {
                string extennsion = Path.GetExtension(fImage.FileName);
                image = Utilities.ToUrlFriendly(product.Name) + extennsion;
                product.Image = await Utilities.UploadFile(fImage, @"products", image.ToLower());
            }
            if (string.IsNullOrEmpty(product.Image))
                product.Image = "product.jpg";

            ViewBag.ProductType = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
            _context.Add(product);
            _notyfService.Success("Thêm thành công!");
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.ProductType = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SKU,Name,Description,ProductTypeId,Price,Stock,Image,Status")] Product product, IFormFile fImage)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            try
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var stringChars = new char[10];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                product.SKU = new String(stringChars);

                product.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(product.Name);

                if (fImage != null)
                {
                    string extennsion = Path.GetExtension(fImage.FileName);
                    image = Utilities.ToUrlFriendly(product.Name) + extennsion;
                    product.Image = await Utilities.UploadFile(fImage, @"products", image.ToLower());
                }
                if (string.IsNullOrEmpty(product.Image))
                    product.Image = "product.jpg";


                ViewBag.ProductType = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
                _context.Update(product);
                _notyfService.Success("Sửa thành công!");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            _notyfService.Success("Xoá thành công!");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
