using btl_tkweb.Data;
using Microsoft.AspNetCore.Mvc;

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
    }
}
