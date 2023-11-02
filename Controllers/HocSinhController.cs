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
        private readonly UserManager<HocSinh> _userManager;
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public HocSinhController(
            UserManager<HocSinh> userManager,
            SignInManager<AccountUser> signInManager,
            SchoolContext db
        ) {
            this.db = db; 
            _userManager = userManager;
            _signInManager = signInManager;
            HocSinh.count = db.HocSinh.Count();
        }
        public IActionResult Index(string LopID)
        {
            var hs = db.HocSinh
                .Where(hs => hs.LopID.Equals(LopID))
                /*.OrderBy(s => s.Ten).OrderBy(s => s.Ho)*/
                .ToList();
            ViewBag.LopID = LopID;
            return View(hs);
        }

        public IActionResult Create(string LopID)
        {
            ViewBag.LopID = LopID;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Ho", "Ten", "Nu", "NgaySinh","LopID", "GhiChu")] HocSinh hs,string LopID)
        {
            ViewBag.LopID = LopID;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                hs.UserName=hs.Username;
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
            if(id == null || db.HocSinh == null)
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
            if(id != hs.Id)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    var user = db.HocSinh.First(u => u.Id == hs.Id);
                    user.HocSinhID = hs.HocSinhID;
                    user.Ho = hs.Ho;
                    user.Ten = hs.Ten;
                    user.Nu = hs.Nu;
                    user.NgaySinh = hs.NgaySinh;
                    user.LopID = hs.LopID;
                    user.GhiChu = hs.GhiChu;
                    db.HocSinh.Update(user);
                    db.SaveChanges();
                    user.UserName = user.Username;
                    await _userManager.UpdateAsync(user);

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
            if(id == null || db.HocSinh == null)
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
            if(db.HocSinh == null)
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
