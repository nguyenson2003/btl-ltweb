using btl_tkweb.Data;
using btl_tkweb.Models;
using Microsoft.AspNetCore.Mvc;
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
    }
}
