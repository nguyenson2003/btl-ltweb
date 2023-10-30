using btl_tkweb.Data;
using btl_tkweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace btl_tkweb.Controllers
{
    public class LopController : Controller
    {
        SchoolContext db;
        public LopController(SchoolContext db) { 
            this.db = db;
        }
        public IActionResult Index()
        {   
            var lop = db.Lop.Include(s=>s.dshs).ToList();
            return View(lop);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("LopID","GVCN")] Lop lop)
        {
            if(ModelState.IsValid)
            {
                db.Lop.Add(lop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (id == null) return NotFound();
            var lop = db.Lop.Find(id);
            if (lop == null){
                return NotFound();
            }
            ViewBag.LopId = id;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string oldid, [Bind("LopID","GVCN")] Lop lop)
        {
            if (oldid == null || db.Lop.Find(oldid)==null){
                return NotFound();
            }
            ViewBag.LopId = oldid;
            if (ModelState.IsValid) {
                
                db.Add(lop);
                db.SaveChanges();
                foreach(var item in db.ChiTietGiangDay)
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
    }
}
