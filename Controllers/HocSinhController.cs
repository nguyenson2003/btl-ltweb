using btl_tkweb.Data;
using btl_tkweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace btl_tkweb.Controllers
{
    public class HocSinhController : Controller
    {
        SchoolContext db;
        public HocSinhController(SchoolContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            var hs = db.HocSinh
                //.Where(hs => hs.LopID.Equals(LopID))
                /*.OrderBy(s => s.Ten).OrderBy(s => s.Ho)*/
                
                .ToList();
            //ViewBag.LopID = LopID;
            return View(hs);
        }

        public IActionResult Create()
        {
            ViewBag.LopID = new SelectList(db.Lop, "LopID", "LopID");
            return View();

        }

        [HttpPost]
        public IActionResult Create([Bind("Ho, Ten, Nu, NgaySinh, LopID, GhiChu")] HocSinh hs)
        {
            if(ModelState.IsValid)
            {
                
                db.HocSinh.Add(hs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LopID = new SelectList(db.Lop, "LopID", "LopID");
            return View();
        }
    }
}
