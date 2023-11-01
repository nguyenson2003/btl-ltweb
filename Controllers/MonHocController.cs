using btl_tkweb.Data;
using btl_tkweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace btl_tkweb.Controllers
{
    public class MonHocController : Controller
    {
        SchoolContext db;
        public MonHocController(SchoolContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            var monhoc = db.MonHoc.ToList();
            return View(monhoc);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("TenMon", "HeSoMon")] MonHoc mon)
        {
            if (ModelState.IsValid)
            {
                db.MonHoc.Add(mon);
                db.SaveChanges();
                foreach(var item in db.HocSinh)
                {
                    db.DiemSo.Add(new DiemSo() { HocSinhId1 = item.HocSinhID, MonHocID = mon.MonHocID});
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }


        public IActionResult Edit(int id)
        {
            if (id == null || db.MonHoc == null) return NotFound();
            var mon = db.MonHoc.Find(id);
            if (mon == null) return NotFound();
            return View(mon);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("MonHocID", "TenMon", "HeSoMon")] MonHoc mon)
        {
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
