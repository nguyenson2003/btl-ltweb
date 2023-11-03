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
            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(ds);
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
                        return Content("" + ds.HocSinhId);
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
