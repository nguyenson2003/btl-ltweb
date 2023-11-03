using btl_tkweb.Data;
using btl_tkweb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace btl_tkweb.Controllers
{
    public class MonHocController : Controller
    {
        private readonly UserManager<AccountUser> _userManager;
        SchoolContext db;
        public MonHocController(SchoolContext db, UserManager<AccountUser> userManager)
        {
            this.db = db;
            _userManager = userManager;
        }
        private AccountUser getUser() { return _userManager.GetUserAsync(HttpContext.User).Result; }
        public IActionResult Index()
        {
            var user = getUser();
            if (user == null) return NotFound();

            var monhoc = db.MonHoc.ToList();
            return View(monhoc);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var user = getUser();
            if (!(user != null && user.role == AccountUser.ADMIN)) return NotFound();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("TenMon", "HeSoMon")] MonHoc mon)
        {
            var user = getUser();
            if (!(user != null && user.role == AccountUser.ADMIN)) return NotFound();

            if (ModelState.IsValid)
            {
                db.MonHoc.Add(mon);
                db.SaveChanges();
                foreach(var item in db.HocSinh)
                {
                    db.DiemSo.Add(new DiemSo() { HocSinhId = item.Id, MonHocID = mon.MonHocID});
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }


        public IActionResult Edit(int id)
        {
            var user = getUser();
            if (!(user != null && user.role == AccountUser.ADMIN)) return NotFound();

            if (id == null || db.MonHoc == null) return NotFound();
            var mon = db.MonHoc.Find(id);
            if (mon == null) return NotFound();
            return View(mon);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("MonHocID", "TenMon", "HeSoMon")] MonHoc mon)
        {
            var user = getUser();
            if (!(user != null && user.role == AccountUser.ADMIN)) return NotFound();

            if (id != mon.MonHocID)
            {
                return Content(id+" "+mon.MonHocID);
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(mon);
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Gvexist(mon.MonHocID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(mon);
        }
        private bool Gvexist(int id)
        {
            return (db.MonHoc?.Any(e => e.MonHocID == id)).GetValueOrDefault();
        }

        public IActionResult Delete(int id)
        {
            var user = getUser();
            if (!(user != null && user.role == AccountUser.ADMIN)) return NotFound();

            if (id == null || db.MonHoc == null)
            {
                return NotFound();
            }
            var mon = db.MonHoc.Include(m=>m.GiaoViens).FirstOrDefault(m => m.MonHocID == id);
            if (mon == null)
            {
                return NotFound();
            }
            if (mon.GiaoViens.Count() > 0)
            {
                return Content("Môn học này còn giáo viên chưa xóa!");
            }
            return View(mon);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = getUser();
            if (!(user != null && user.role == AccountUser.ADMIN)) return NotFound();

            if (db.MonHoc == null)
            {
                return Problem("Không còn môn học");
            }
            var mon = db.MonHoc.Find(id);
            if (mon != null)
            {
                db.MonHoc.Remove(mon);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
