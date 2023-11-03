using btl_tkweb.Data;
using btl_tkweb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace btl_tkweb.Controllers
{
    public class DiemSoController : Controller
    {
        private readonly UserManager<AccountUser> _userManager;
        SchoolContext db;
        public DiemSoController(SchoolContext db,UserManager<AccountUser> userManager)
        {
            _userManager = userManager;
            this.db = db;
        }
        private AccountUser getUser() { return _userManager.GetUserAsync(HttpContext.User).Result; }
        public IActionResult Index(string HocSinhID)
        {
            var user = getUser();
            if (user == null) return NotFound();

            if (user.role == AccountUser.ADMIN)
            {
                var bd = db.DiemSo.Include(m => m.MonHoc).Include(l => l.HocSinh).Where(s => s.HocSinhId.Equals(HocSinhID)).ToList();
                var hs = bd.First().HocSinh;
                ViewBag.HocSinh = hs.Ho + hs.Ten;
                return View(bd);
            }
            if (user.role == AccountUser.GIAOVIEN) { 
                var bd = db.DiemSo
                    .Include(m => m.MonHoc)
                    .Include(l => l.HocSinh)
                    .Where(s => s.HocSinhId.Equals(HocSinhID))
                    .Where(bd=>bd.MonHocID==((GiaoVien)user).MonHocID).ToList();
                var hs = bd.First().HocSinh;
                
                ViewBag.HocSinh = hs.Ho + hs.Ten;
                return View(bd);
            }
            if (user.role == AccountUser.HOCSINH)
            {
                var bd = db.DiemSo.Include(m => m.MonHoc).Include(l => l.HocSinh).Where(s => s.HocSinhId.Equals(HocSinhID)).ToList();
                var hs = bd.First().HocSinh;
                if (hs.Id!=user.Id) return NotFound();
                ViewBag.HocSinh = hs.Ho + hs.Ten;
                return View(bd);
            }
            return NotFound();


        }

        public IActionResult Edit(int id)
        {
            if(id == null || db.DiemSo == null)
            {
                return NotFound();
            }
            var user = getUser();
            if(user == null) { return NotFound(); }
            if(user.role==AccountUser.HOCSINH) { return NotFound(); }
            var bd = db.DiemSo.Find(id);
            if(user.role==AccountUser.GIAOVIEN && 
                !(bd.MonHocID==((GiaoVien)user).MonHocID && db.ChiTietGiangDay.Where(c=>c.GiaoVienID==user.Id).Where(c=>c.LopId == db.HocSinh.Where(hs=>hs.Id==bd.HocSinhId).First().LopID).Count()!=0)
            ) return NotFound();
            if(bd == null)
            {
                return NotFound();
            }
            return View(bd);
        }

        [HttpPost]

        public IActionResult Edit(int id, [Bind("DiemSoID", "MonHocID", "HocSinhId", "DiemHeSo1_1", "DiemHeSo1_3", "DiemHeSo1_2", "DiemHeSo2_1", "DiemHeSo2_2", "DiemHeSo3")] DiemSo ds)
        {
            if (id != ds.DiemSoID)
            {
                return NotFound();
            }
            var user = getUser();
            if (user == null) { return NotFound(); }
            if (user.role == AccountUser.HOCSINH) { return NotFound(); }
            var bd = db.DiemSo.Find(id);
            if (user.role == AccountUser.GIAOVIEN &&
                !(bd.MonHocID == ((GiaoVien)user).MonHocID && db.ChiTietGiangDay.Where(c => c.GiaoVienID == user.Id).Where(c => c.LopId == db.HocSinh.Where(hs => hs.Id == bd.HocSinhId).First().LopID).Count() != 0)
            ) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var userbd = db.DiemSo.Find(id);
                    userbd.DiemHeSo1_1 = ds.DiemHeSo1_1;
                    userbd.DiemHeSo1_2 = ds.DiemHeSo1_2;
                    userbd.DiemHeSo1_3 = ds.DiemHeSo1_3;
                    userbd.DiemHeSo2_1 = ds.DiemHeSo2_1;
                    userbd.DiemHeSo2_2 = ds.DiemHeSo2_2;
                    userbd.DiemHeSo3 = ds.DiemHeSo3;
                    db.DiemSo.Update(userbd);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    if (!DSexist(ds.DiemSoID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new {ds.HocSinhId});
            }
            
            return View(ds);
        }
        private bool DSexist(int id)
        {
            return (db.DiemSo?.Any(e => e.DiemSoID == id)).GetValueOrDefault();
        }

    }
}
