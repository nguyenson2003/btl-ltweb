using btl_tkweb.Data;
using btl_tkweb.Models;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index(string LopID)
        {
            var hs = db.HocSinh
                .Where(s => s.LopID.Equals(LopID))
                /*.OrderBy(s => s.Ten).OrderBy(s => s.Ho)*/
                .ToList();
            ViewBag.LopID = LopID;
            return View(hs);
        }
    }
}
