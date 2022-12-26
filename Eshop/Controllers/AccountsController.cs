using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Eshop.Helpper;
using Eshop.Extension;
using System.Security.Claims;
using System.Security.Principal;
using System.Globalization;
using Eshop.ViewModel;

namespace Eshop.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly EshopContext _context;
        public INotyfService _notyfService { get; }

        public static string image;

        public AccountsController(EshopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Accounts.ToListAsync());
        }

        #region DangKy
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewBag.lsTypes = _context.ProductTypes.ToList();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,Username,Password,ConfirmPassword,Email,Phone,Address,FullName,IsAdmin,Avatar,Status,RandomKey")] Account account, IFormFile fAvatar)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string RandomKey = Utilities.GetRandomKey();
                    account.RandomKey = RandomKey;
                    account.Password = (account.Password + RandomKey.Trim()).PassToMD5();
                    account.Status = true;

                    if (fAvatar != null)
                    {
                        string extennsion = Path.GetExtension(fAvatar.FileName);
                        image = Utilities.ToUrlFriendly(account.FullName) + extennsion;
                        account.Avatar = await Utilities.UploadFile(fAvatar, @"User", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(account.Avatar))
                    {
                        account.Avatar = "avatar.png";
                    }
                    try
                    {
                        _context.Add(account);
                        _notyfService.Success("Tạo thành công!");
                        await _context.SaveChangesAsync();

                        HttpContext.Session.SetString("AccountId", account.Id.ToString());
                        var AccountId = HttpContext.Session.GetString("AccountId");

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, account.FullName),
                            new Claim("AccountId", account.Id.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);

                        return RedirectToAction("Index", "Home");
                    }
                    catch
                    {
                        _notyfService.Error("Lỗi khi tạo tài khoản! Chuyển hướng về trang Đăng Ký");
                        return RedirectToAction("Register", "Accounts");
                    }
                }
                else
                {
                    return View(account);
                }
            }
            catch
            {
                return View(account);
            }
        }
        #endregion

        #region DangNhap
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.lsTypes = _context.ProductTypes.ToList();
            var AccountId = HttpContext.Session.GetString("AccountId");
            if (AccountId != null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnURL = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Account account)
        {
            try
            {
                var _account = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Username.Trim() == account.Username);
                
                if (_account == null)
                {
                    _notyfService.Error("Thông tin đăng nhập không chính xác");
                    return RedirectToAction("Login", "Accounts");
                }
                string pass = (account.Password + _account.RandomKey.Trim()).PassToMD5();

                if (_account.Password != pass)
                {
                    _notyfService.Error("Thông tin đăng nhập không chính xác");
                    return View(_account);
                }

                if (_account.Status == false)
                {
                    _notyfService.Warning("Tài khoản của bạn đang bị khóa, vui lòng liên hệ Admin!");
                    return RedirectToAction("Login", "Accounts");
                }

                if (_account.IsAdmin == true)
                {
                    _notyfService.Custom("Đăng nhập thành công!", 3, "#EAB14E", "fas fa-crown");
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }

                HttpContext.Session.SetString("AccountId", _account.Id.ToString());

                var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, _account.FullName),
                            new Claim("AccountId", _account.Id.ToString())
                        };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Login");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
                _notyfService.Custom("Đăng nhập thành công!", 3, "#EAB14E", "fas fa-crown");
                return RedirectToAction("Index", "Home");

            }
            catch
            {
                _notyfService.Error("Thông tin đăng nhập không chính xác");
                return RedirectToAction("Login", "Accounts");
            }
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
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion

        #region MyAccount
        // GET: Accounts/Details/5
        public IActionResult MyAccount()
        {
            ViewBag.lsTypes = _context.ProductTypes.ToList();
            var accountId = HttpContext.Session.GetString("AccountId");

            if (accountId != null)
            {
                var _accountId = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Id == Convert.ToInt32(accountId));

                if (_accountId != null)
                {
                    var lstInvoice = _context.Invoices.AsNoTracking().Where(x => x.AccountId == _accountId.Id).OrderByDescending(x => x.IssuedDate).ToList();
                    ViewBag.ListDonHang = lstInvoice;
                    return View(_accountId);
                }
                return View(_accountId);
            }
            return RedirectToAction("Login", "Accounts");
        }
        #endregion

        #region EditTaiKhoan
        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.lsTypes = _context.ProductTypes.ToList();
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var accountId = HttpContext.Session.GetString("AccountId");
            if (id != Convert.ToInt32(accountId))
            {
                return RedirectToAction("Logout", "Accounts");
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }


        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,Email,Phone,Address,FullName,IsAdmin,Avatar,Status,RandomKey")] Account account, IFormFile fAvatar)
        {
            if (id != account.Id)
            {
                return NotFound();
            }
            try
            {
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
            return RedirectToAction(nameof(MyAccount));
        }
        #endregion

        #region DoiMatKhau
        [HttpGet]
        public IActionResult ChangePassword()
        {
            ViewBag.lsTypes = _context.ProductTypes.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel _user)
        {
            try
            {
                var accountId = HttpContext.Session.GetString("AccountId");
                if (accountId == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }
                if (ModelState.IsValid)
                {
                    var _accountId = _context.Accounts.Find(Convert.ToInt32(accountId));
                    if (_accountId == null)
                        return RedirectToAction("Login", "Accounts");

                    var pass = (_user.CurrentPassword.Trim() + _accountId.RandomKey.Trim()).PassToMD5();
                    if (pass == _accountId.Password)
                    {
                        string newpass = (_user.Password.Trim() + _accountId.RandomKey.Trim()).PassToMD5();
                        _accountId.Password = newpass;
                        _context.Update(_accountId);
                        _notyfService.Success("Cập nhật thành công");
                        _context.SaveChanges();
                        return RedirectToAction("MyAccount", "Accounts");
                    }
                }
            }
            catch
            {
                _notyfService.Error("Update không thành công");
                return RedirectToAction("ChangePassword", "Accounts");
            }
            _notyfService.Error("Update không thành công");
            return RedirectToAction("ChangePassword", "Accounts");
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
