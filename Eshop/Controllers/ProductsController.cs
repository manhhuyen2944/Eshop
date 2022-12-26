using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using X.PagedList;

namespace Eshop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly EshopContext _context;

        public ProductsController(EshopContext context)
        {
            _context = context;
        }

        // GET: Products
        public IActionResult Index(int? page)
        {
            ViewBag.lsTypes = _context.ProductTypes.ToList();
            //list sp
            var lstSanPham = _context.Products
                                    .AsNoTracking()
                                    .Include(a => a.ProductType)
                                    .Where(a => a.Status && a.Stock > 0)
                                    .OrderByDescending(a => a.Id);


            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 9;
            PagedList<Product> model = new PagedList<Product>(lstSanPham, pageNo, pageSize);
            ViewBag.CurrentPage = pageNo;
            ViewBag.ListType = _context.ProductTypes.ToPagedList();

            return View(model);
        }

        public IActionResult Search(string searchStr, int? page)
        {
            ViewBag.lsTypes = _context.ProductTypes.ToList();

            var _product = from m in _context.Products.Include(p => p.ProductType) select m;

            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                _product = _product.Where(p => p.Name.Contains(searchStr) || p.Id.ToString().Contains(searchStr));
            }

            ViewBag.ListType = _context.ProductTypes.ToPagedList();

            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 3;
            PagedList<Product> models = new PagedList<Product>(_product, pageNo, pageSize);
            ViewBag.CurrentPage = pageNo;

            return View(models);
        }

        public IActionResult ProductType(int id, int? page)
        {
            ViewBag.lsTypes = _context.ProductTypes.ToList();


            var lst = _context.Products.Include(a => a.ProductType)
                .Where(a => a.ProductTypeId == id).ToPagedList();

          //  ViewBag.ListType = _context.ProductTypes.ToPagedList();

            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 9;
            PagedList<Product> models = new PagedList<Product>(lst, pageNo, pageSize);
            ViewBag.CurrentPage = pageNo;

            return View("Index", lst);
        }

        


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.lsTypes = _context.ProductTypes.ToList();
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            return View(product);
        }
    }
}
