using AspNetCoreHero.ToastNotification.Abstractions;
using Eshop.Data;
using Eshop.Extension;
using Eshop.Helpper;
using Eshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Security.Principal;
using X.PagedList;

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : Controller
    {
        private readonly EshopContext _context;
        public static string image;
        public INotyfService _notyfService { get;}
        public AccountsController(EshopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        public IActionResult Index(string sortOrder, string currentFilter, string searchStr, int? page)
        {
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["CurrentSort"] = sortOrder;

            var _customer = from m in _context.Accounts select m;

            switch (sortOrder)
            {
                case "id_desc":
                    _customer = _customer.OrderByDescending(x => x.Id);
                    break;
                case "Name":
                    _customer = _customer.OrderBy(p => p.FullName);
                    break;
                case "name_desc":
                    _customer = _customer.OrderByDescending(p => p.FullName);
                    break;
                default:
                    _customer = _customer.OrderBy(x => x.Id);
                    break;
            }

            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                _customer = _customer.Where(s => s.FullName.Contains(searchStr) || s.Id.ToString().Contains(searchStr) || s.Email.Contains(searchStr) || s.Phone.Contains(searchStr) || s.Address.Contains(searchStr));
            }

            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;
            PagedList<Account> models = new PagedList<Account>(_customer, pageNo, pageSize);

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

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }
        #endregion

        #region Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,FullName,IsAdmin,Avatar,Address,Email,Phone,Status,RandomKey")] Account account, IFormFile fAvatar)
        {
            account.FullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(account.FullName);
            string RandomKey = Utilities.GetRandomKey();
            if (fAvatar != null)
            {
                string extennsion = Path.GetExtension(fAvatar.FileName);
                image = Utilities.ToUrlFriendly(account.FullName) + extennsion;
                account.Avatar = await Utilities.UploadFile(fAvatar, @"User", image.ToLower());
            }
            if (string.IsNullOrEmpty(account.Avatar))
                account.Avatar = "avatar.png";

            account.Password = (account.Password + RandomKey.Trim()).PassToMD5();
            account.RandomKey = RandomKey;
            _context.Add(account);
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

            var customer = await _context.Accounts.FindAsync(id);

            //customer.Password = (customer.Password + customer.RandomKey.Trim()).PassToMD5();

            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,FullName,IsAdmin,Avatar,Address,Email,Phone,Status,RandomKey")] Account account, IFormFile fAvatar, String newpassword)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            try
            {
                var _custommer = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Id == account.Id);

                if(!String.IsNullOrEmpty(newpassword))
                {
                    string newpass = (newpassword.Trim() + _custommer.RandomKey.Trim()).PassToMD5();
                    account.Password = newpass;
                }
                //string newpass = (account.Password.Trim() + _custommer.RandomKey.Trim()).PassToMD5();
                //account.Password = newpass;

                account.FullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(account.FullName);
                if (fAvatar != null)
                {
                    string extennsion = Path.GetExtension(fAvatar.FileName);
                    image = Utilities.ToUrlFriendly(account.FullName) + extennsion;
                    account.Avatar = await Utilities.UploadFile(fAvatar, @"User", image.ToLower());
                }
                if (string.IsNullOrEmpty(account.Avatar))
                    account.Avatar = "avatar.png";

                _notyfService.Success("Sửa thành công!");
                _context.Update(account);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(account.Id))
                {
                    _notyfService.Error("Lỗi!!!!!!!!!!!!");
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

        #region Kiemtratrung
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var _account = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone);
                if (_account != null)
                {
                    return Json(data: "Số : " + Phone + " này đã được sử dụng");
                }
                return Json(data: true);
            }
            catch
            {

                return Json(data: true);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var _account = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
                if (_account != null)
                {
                    return Json(data: "Email : " + Email + " này đã được sử dụng");
                }
                return Json(data: true);
            }
            catch
            {

                return Json(data: true);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateUserName(string Username)
        {
            try
            {
                var _account = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Username.ToLower() == Username.ToLower());
                if (_account != null)
                {
                    return Json(data: "Tên tài khoản : " + Username + " này đã được sử dụng");
                }
                return Json(data: true);
            }
            catch
            {

                return Json(data: true);
            }
        }
        #endregion
        
        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
