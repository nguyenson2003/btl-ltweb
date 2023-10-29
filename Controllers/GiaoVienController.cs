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
            return View(gv);
        }
        public IActionResult Table(string LopID, int? MonHocID)
        {
            if (LopID == null && MonHocID == null)
            {
                var gv = db.GiaoVien.Include(s => s.MonHoc).ToList();
                return PartialView(gv);
            }
            else if (LopID == null)
            {
                var gv = db.GiaoVien.Include(s => s.MonHoc).Where(s => s.MonHocID == MonHocID);
                return PartialView(gv);
            }
            else if (MonHocID == null)
            {
                var gv = db.GiaoVien.Include(s => s.MonHoc).Include(s => s.ctgd).Where(s => s.ctgd.Any(c => c.LopHocId == LopID));
                return PartialView(gv);
            }
            else
            {
                var gv = db.GiaoVien.Include(s => s.MonHoc).Include(s => s.ctgd).Where(s => s.ctgd.Any(c => c.LopHocId == LopID)).Where(s => s.MonHocID == MonHocID);
                return PartialView(gv);
            }
        }
    }
}
