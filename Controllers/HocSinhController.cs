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
        private readonly IUserStore<AccountUser> _userStore;
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public HocSinhController(
            UserManager<AccountUser> userManager,
            IUserStore<AccountUser> userStore,
            SignInManager<AccountUser> signInManager,
            SchoolContext db
        ) {
            this.db = db; 
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
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
                db.HocSinh.Add(hs);
                db.SaveChanges();
                hs.createBangDiem(db);
                
                var user = new AccountUser{HocSinh = hs};

                await _userStore.SetUserNameAsync(user, hs.Username, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, "Utc@123");

                if (result.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    return RedirectToAction("Index",new {LopID});
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        public IActionResult Edit(int id)
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
        public IActionResult Edit(int id, [Bind("HocSinhID", "Ho", "Ten", "Nu", "NgaySinh","LopID", "GhiChu")] HocSinh hs)
        {
            if(id != hs.HocSinhID)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    db.Update(hs);
                    db.SaveChanges();
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


        public IActionResult Delete(int id)
        {
            if(id == null || db.HocSinh == null)
            {
                return NotFound();
            }
            var hs = db.HocSinh.Include(l => l.Lop).Include(e => e.DiemSo).FirstOrDefault(m => m.HocSinhID == id);
            if(hs == null)
            {
                return NotFound();
            }
            return View(hs);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
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
