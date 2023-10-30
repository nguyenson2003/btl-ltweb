using btl_tkweb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace btl_tkweb.Controllers
{
    public class DiemSoController : Controller
    {
        SchoolContext db;
        public DiemSoController(SchoolContext db)
        {
            this.db = db;
        }
        public IActionResult Index(String HocSinhID)
        {
            var bd = db.DiemSo.Include(l => l.HocSinh).Where(s => s.HocSinhId.Equals(HocSinhID)).ToList();
            var hs = db.DiemSo.Find(HocSinhID);
            ViewBag.HocSinh = hs.HocSinh.Ho + hs.HocSinh.Ten;
            return View(bd);
        }
       
    }
}
