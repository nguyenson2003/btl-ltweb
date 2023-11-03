using btl_tkweb.Data;
using btl_tkweb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace btl_tkweb.Controllers
{
    public class LopController : Controller
    {
        private readonly UserManager<AccountUser> _userManager;
        SchoolContext db;
        public LopController(SchoolContext db, UserManager<AccountUser> userManager)
        {
            this.db = db;
            _userManager = userManager;
        }
        private AccountUser getUser() { return _userManager.GetUserAsync(HttpContext.User).Result; }

        public IActionResult Index()
        {
            var user = getUser();
            if (user == null) return NotFound();
            if (user.role == AccountUser.ADMIN)
            {
                var lop = db.Lop.Include(s => s.dshs).ToList();
                return View(lop);
            }
            if (user.role == AccountUser.GIAOVIEN)
            {
                var lop = db.Lop.Include(s => s.dshs).Include(l=>l.ctgd).Where(l=>l.ctgd.Any(ct=>ct.GiaoVienID==((GiaoVien)user).Id)).ToList();
                return View(lop);
            }
            if (user.role == AccountUser.HOCSINH)
            {
                var lop = db.Lop.Include(s => s.dshs).Where(l=>l.LopID==((HocSinh)user).LopID);
                return View(lop);
            }
            return NotFound();
        }
        [HttpGet]
        public IActionResult Create()
        {
            var user = getUser();
            if(user!=null && user.role==AccountUser.ADMIN)
                return View();
            else
                return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("LopID", "GVCN")] Lop lop)
        {
            var user = getUser();
            if (user != null && user.role == AccountUser.ADMIN)
            {
                if (ModelState.IsValid)
                {
                    db.Lop.Add(lop);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View();
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var user = getUser();
            if (user != null && user.role == AccountUser.ADMIN)
            {
                if (id == null) return NotFound();
                var lop = db.Lop.Find(id);
                if (lop == null)
                {
                    return NotFound();
                }
                ViewBag.LopId = id;
                return View();
            }
            return NotFound();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string oldid, [Bind("LopID","GVCN")] Lop lop)
        {

            var user = getUser();
            if (user != null && user.role == AccountUser.ADMIN)
            {
                if (oldid == null || db.Lop.Find(oldid) == null)
                {
                    return NotFound();
                }
                ViewBag.LopId = oldid;
                if (ModelState.IsValid)
                {

                    db.Add(lop);
                    db.SaveChanges();
                    foreach (var item in db.ChiTietGiangDay)
                    {
                        if (item.LopHocId == oldid)
                        {
                            item.LopHocId = lop.LopID;
                            db.Update(item);
                        }
                    }
                    db.SaveChanges();
                    foreach (var item in db.HocSinh)
                    {
                        if (item.LopID == oldid)
                        {
                            item.LopID = lop.LopID;
                            db.Update(item);
                        }
                    }
                    db.SaveChanges();
                    db.Lop.Remove(db.Lop.Find(oldid));
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View();
            }
            return NotFound();
        }
        [HttpGet]
        public IActionResult Delete(string id)
        {

            var user = getUser();
            if (user != null && user.role == AccountUser.ADMIN)
            {
                if (id == null) return NotFound();
                var lop = db.Lop.Find(id);
                if (lop == null) return NotFound();
                return View(lop);
            }
            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(string id)
        {

            var user = getUser();
            if (user != null && user.role == AccountUser.ADMIN)
            {
                if (id == null) return NotFound();
                var lop = db.Lop.Include(l => l.dshs).Where(l => l.LopID == id);
                if (lop.Count() == 0) return NotFound();
                foreach (var item in lop.First().dshs)
                {
                    item.deleteBangDiem(db);
                    db.SaveChanges();
                    db.Remove(item);
                    db.SaveChanges();
                }
                foreach (var item in lop.First().ctgd)
                {
                    db.Remove(item);
                }
                db.Lop.Remove(lop.First());
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}
