using btl_tkweb.Data;
using btl_tkweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace btl_tkweb.Controllers
{
    public class GiaoVienController : Controller
    {
        SchoolContext db;
        public GiaoVienController(SchoolContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            var gv = db.GiaoVien.Include(s => s.MonHoc).ToList();
            var lop = new List<Lop>();
            lop.Add(new Lop() { LopID = "" });
            lop.AddRange(db.Lop);
            ViewBag.lopId = new SelectList(lop, "LopID","LopID","");

            var mon= new List<MonHoc>();
            mon.Add(new MonHoc() { MonHocID= 0 ,TenMon="--",HeSoMon=0});
            mon.AddRange(db.MonHoc);
            ViewBag.MonHoc = new SelectList(mon, "MonHocID", "TenMon", "--");

            return View(gv);
        }
        public PartialViewResult Table(string LopID, int? MonHocID)
        {
            if (LopID == "--") LopID = null;
            if(MonHocID==0)MonHocID=null;
            if (LopID == null && MonHocID == null)
            {
                var gv = db.GiaoVien.Include(s => s.MonHoc).ToList();
                return PartialView(gv);
            }
            else if (LopID == null)
            {
                var gv = db.GiaoVien.Include(s => s.MonHoc).Where(s => s.MonHocID == MonHocID).ToList();
                return PartialView(gv);
            }
            else if (MonHocID == null)
            {
                var gv = db.GiaoVien.Include(s => s.MonHoc).Include(s => s.ctgd).Where(s => s.ctgd.Any(c => c.LopHocId == LopID)).ToList();
                return PartialView(gv);
            }
            else
            {
                var gv = db.GiaoVien.Include(s => s.MonHoc).Include(s => s.ctgd).Where(s => s.ctgd.Any(c => c.LopHocId == LopID)).Where(s => s.MonHocID == MonHocID).ToList();
                return PartialView(gv);
            }
        }


        public IActionResult Create()
        {
            ViewBag.MonHocID = new SelectList(db.MonHoc, "MonHocID", "TenMon", "");
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("HoVaTen", "Nu","NgaySinh", "MonHocID","GhiChu")] GiaoVien gv)
        {
        
            if(ModelState.IsValid)
            {

                db.GiaoVien.Add(gv);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View();
            
        }

        public IActionResult Edit(int id)
        {
            if (id == null || db.GiaoVien == null)
            {
                return NotFound();
            }
            var gv = db.GiaoVien.Find(id);
            if (gv == null)
            {
                return NotFound();
            }
            ViewBag.MonHocID = new SelectList(db.MonHoc, "MonHocID", "TenMon", gv.MonHocID);
            return View(gv);
        }

        [HttpPost]

        public IActionResult Edit(int id, [Bind("GiaoVienID", "HoVaTen", "Nu", "NgaySinh", "MonHocID", "GhiChu")] GiaoVien gv)
        {
            if (id != gv.GiaoVienID)
            {
                return Content(id + " "+ gv.GiaoVienID);

                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(gv);
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Gvexist(gv.GiaoVienID))
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
            ViewBag.MonHocID = new SelectList(db.MonHoc, "MonHocID", "TenMon", gv.MonHocID);
            return View(gv);
        }
        private bool Gvexist(int id)
        {
            return (db.GiaoVien?.Any(e => e.GiaoVienID == id)).GetValueOrDefault();
        }

        public IActionResult Delete(int id)
        {
            if (id == null || db.GiaoVien == null)
            {
                return NotFound();
            }
            var gv = db.GiaoVien.Include(l => l.MonHoc).Include(e => e.ctgd).FirstOrDefault(m => m.GiaoVienID == id);
            if (gv == null)
            {
                return NotFound();
            }
            if (gv.ctgd.Count() > 0)
            {
                return Content("Giáo viên còn chi tiết chưa xóa!");
            }
            return View(gv);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (db.GiaoVien == null)
            {
                return Problem("Khong con giao vien");
            }
            var gv = db.GiaoVien.Find(id);
            

            if (gv != null)
            {
                db.GiaoVien.Remove(gv);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
