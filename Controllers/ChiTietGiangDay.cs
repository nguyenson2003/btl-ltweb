using btl_tkweb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace btl_tkweb.Controllers
{
    public class ChiTietGiangDay : Controller
    {
        SchoolContext db;

        public ChiTietGiangDay(SchoolContext db)
        {
            this.db = db;
        }
        public IActionResult Index(int GiaoVienID)
        {
            var gd = db.ChiTietGiangDay.Include(m => m.Lop).Include(n => n.GiaoVien).Where(l => l.GiaoVienID == GiaoVienID).ToList();
            var gv = gd.First().GiaoVien;
            ViewBag.GiaoVien = gv?.HoVaTen;
            return View(gd);
        }


        public IActionResult Create(int GiaoVienID)
        {
            ViewBag.GiaoVienID = GiaoVienID;
            ViewBag.Lop = new SelectList(db.Lop, "LopID", "LopID");
            return View();

        }

        [HttpPost]
        public IActionResult Create([Bind("GiaoVienID", "LopHocID")] ChiTietGiangDay ct, string GiaoVienID)
        {
            ViewBag.GiaoVienID = GiaoVienID;
            if (ModelState.IsValid)
            {
                db.HocSinh.Add(ct);
                db.SaveChanges();
                
                return RedirectToAction("Index", new { GiaoVienID });
            }
            ViewBag.Lop = new SelectList(db.Lop, "LopID", "LopID");
            return View();
        }

        public IActionResult Delete(int id)
        {
            if (id == null || db.ChiTietGiangDay == null)
            {
                return NotFound();
            }
            var gv = db.ChiTietGiangDay.Include(l => l.Lop).Include(e => e.GiaoVien).FirstOrDefault(m => m.GiaoVienID == id);
            if (gv == null)
            {
                return NotFound();
            }
            return View(gv);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (db.ChiTietGiangDay == null)
            {
                return Problem("Khong con chi tiet de xoa");
            }
            var ct = db.ChiTietGiangDay.Find(id);
            var GiaoVienID = ct.GiaoVienID;

            if (ct != null)
            {
                
                db.ChiTietGiangDay.Remove(ct);

            }
            else
            {

            }
            db.SaveChanges();
            return RedirectToAction("Index", new { GiaoVienID });
        }


    }
}
