using btl_tkweb.Data;
using btl_tkweb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace btl_tkweb.Controllers
{
    public class HocSinhController : Controller
    {
        SchoolContext db; 
        private readonly SignInManager<AccountUser> _signInManager;
        private readonly UserManager<AccountUser> _userManager;
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public HocSinhController(
            UserManager<AccountUser> userManager,
            SignInManager<AccountUser> signInManager,
            SchoolContext db
        ) {
            this.db = db; 
            _userManager = userManager;
            _signInManager = signInManager;
            HocSinh.count = db.HocSinh.Count();
        }
        private AccountUser getUser() { return _userManager.GetUserAsync(HttpContext.User).Result; }
        public IActionResult Index(string LopID)
        {
            var user = getUser();
            if(user == null)return NotFound();
            if (user.role == AccountUser.HOCSINH && ((HocSinh)user).LopID != LopID) return NotFound();
            if(user.role == AccountUser.GIAOVIEN && db.ChiTietGiangDay.Where(ct=>ct.LopId==LopID&&ct.GiaoVienID==user.Id).Count()==0) return NotFound();
            var hs = db.HocSinh
                .Where(hs => hs.LopID.Equals(LopID))
                .OrderBy(s => s.Ten).OrderBy(s => s.Ho)
                .ToList();
            ViewBag.LopID = LopID;
            return View(hs);
        }

        public IActionResult Create(string LopID)
        {
            var user = getUser();
            if (!(user != null && user.role == AccountUser.ADMIN)) return NotFound();
            
            ViewBag.LopID = LopID;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Ho", "Ten", "Nu", "NgaySinh","LopID", "GhiChu")] HocSinh hs,string LopID)
        {
            var user = getUser();
            if (!(user != null && user.role == AccountUser.ADMIN)) return NotFound();

            ViewBag.LopID = LopID;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                hs.UserName = hs.Username;
                var result = await _userManager.CreateAsync(hs, "Utc@123");

                if (result.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(hs);
                    hs.createBangDiem(db);
                    return RedirectToAction("Index",new {LopID});
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        public IActionResult Edit(string id)
        {
            var user = getUser();
            if (!(
                (user != null && user.role == AccountUser.ADMIN) ||
                (user != null && user.role == AccountUser.HOCSINH && ((HocSinh)user).Id==id)
            )) return NotFound();

            if (id == null || db.HocSinh == null)
            {
                return NotFound();
            }
            var hs = db.HocSinh.Find(id);
            if(hs == null)
            {
                return NotFound();
            }
            ViewBag.LopID = new SelectList(db.Lop, "LopID", "LopID", hs.LopID);
            return View(hs);
        }


        [HttpPost]
        public async Task<IActionResult> EditAsync(string id, [Bind("Id","HocSinhID", "Ho", "Ten", "Nu", "NgaySinh","LopID", "GhiChu")] HocSinh hs)
        {
            var user = getUser();
            if (!(
                (user != null && user.role == AccountUser.ADMIN) ||
                (user != null && user.role == AccountUser.HOCSINH && ((HocSinh)user).Id == id)
            )) return NotFound();

            if (id != hs.Id)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    var userhs = db.HocSinh.First(u => u.Id == hs.Id);
                    userhs.HocSinhID = hs.HocSinhID;
                    userhs.Ho = hs.Ho;
                    userhs.Ten = hs.Ten;
                    userhs.Nu = hs.Nu;
                    userhs.NgaySinh = hs.NgaySinh;
                    userhs.LopID = hs.LopID;
                    userhs.GhiChu = hs.GhiChu;
                    db.HocSinh.Update(userhs);
                    db.SaveChanges();
                    userhs.UserName = userhs.Username;
                    await _userManager.UpdateAsync(userhs);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if(!Hsexist(hs.HocSinhID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new {hs.LopID});
            }
            ViewBag.LopID = new SelectList(db.Lop, "LopID", "LopID", hs.LopID);
            return View(hs);
        }
        private bool Hsexist(int id)
        {
            return (db.HocSinh?.Any(e => e.HocSinhID == id)).GetValueOrDefault();
        }


        public IActionResult Delete(string id)
        {
            var user = getUser();
            if (!(user != null && user.role == AccountUser.ADMIN)) return NotFound();

            if (id == null || db.HocSinh == null)
            {
                return NotFound();
            }
            var hs = db.HocSinh.Include(l => l.Lop).Include(e => e.DiemSo).FirstOrDefault(m => m.Id == id);
            if(hs == null)
            {
                return NotFound();
            }
           
            return View(hs);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(string id)
        {
            var user = getUser();
            if (!(user != null && user.role == AccountUser.ADMIN)) return NotFound();

            if (db.HocSinh == null)
            {
                return Problem("Khong con hoc sinh");
            }
            var hs = db.HocSinh.Find(id);
            var lopID = hs.LopID;
            
            if(hs != null)
            {
                hs.deleteBangDiem(db);
                db.HocSinh.Remove(hs);

            }
            db.SaveChanges();
            return RedirectToAction("Index" ,new {lopID});
        }
        
    }
}
