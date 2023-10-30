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
    }
}
