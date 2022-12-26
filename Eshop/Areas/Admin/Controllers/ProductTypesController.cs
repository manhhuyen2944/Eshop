using AspNetCoreHero.ToastNotification.Abstractions;
using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using X.PagedList;

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductTypesController : Controller
    {
        private readonly EshopContext _context;
        public INotyfService _notyfService { get; }

        public ProductTypesController(EshopContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }

        public IActionResult Index(string sortOrder, string currentFilter, string searchStr, int? page)
        {
            var _proType = from m in _context.ProductTypes select m;

            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["CurrentSort"] = sortOrder;
            switch (sortOrder)
            {
                case "id_desc":
                    _proType = _proType.OrderByDescending(p => p.Id);
                    break;
                case "Name":
                    _proType = _proType.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    _proType = _proType.OrderByDescending(p => p.Name);
                    break;
                case "Disable":
                    _proType = _proType.OrderBy(p => p.Status);
                    break;
                case "Publish":
                    _proType = _proType.OrderByDescending(p => p.Status);
                    break;
                default:
                    _proType = _proType.OrderBy(p => p.Id);
                    break;
            }

            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                _proType = _proType.Where(p => p.Id.ToString().Contains(searchStr) || p.Name.Contains(searchStr));
            }

            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 9;
            ViewBag.CurrentPage = pageNo;
            PagedList<ProductType> models = new PagedList<ProductType>(_proType, pageNo, pageSize);

            return View(models);
        }

        #region Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Status")] ProductType productType)
        {
            if (ModelState.IsValid)
            {
                productType.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(productType.Name);
                productType.Status = true;

                _context.Add(productType);
                _notyfService.Success("Thêm thành công!");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productType);
        }
        #endregion


        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _context.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }
            return View(productType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name, Status")] ProductType productType)
        {
            if (id != productType.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    productType.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(productType.Name);
                    _context.Update(productType);
                    _notyfService.Success("Sửa thành công!");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductTypeExists(productType.Id))
                    {
                        _notyfService.Error("Lỗi!!!!!!!!!!!!!");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productType);
        }
        #endregion


        private bool ProductTypeExists(int id)
        {
            return _context.ProductTypes.Any(e => e.Id == id);
        }
    }
}
