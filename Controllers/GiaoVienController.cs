using btl_tkweb.Data;
using btl_tkweb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace btl_tkweb.Controllers
{
    public class GiaoVienController : Controller
    {
        SchoolContext db;
        private readonly SignInManager<AccountUser> _signInManager;
        private readonly UserManager<GiaoVien> _userManager;
        private readonly IUserStore<GiaoVien> _userStore; 
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public GiaoVienController(
            UserManager<GiaoVien> userManager,
            IUserStore<GiaoVien> userStore,
            SignInManager<AccountUser> signInManager,
            SchoolContext db
        )
        {
            this.db = db;
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            GiaoVien.count = db.GiaoVien.Count();
        }

        public IActionResult Index(bool? alert)
        {
           if(alert == null) alert = false;
            ViewBag.Alert = alert;
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


        public IActionResult Create()
        {
            ViewBag.MonHocID = new SelectList(db.MonHoc, "MonHocID", "TenMon", "");
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HoVaTen", "Nu","NgaySinh", "MonHocID","GhiChu")] GiaoVien gv)
        {
        
            if(ModelState.IsValid)
            {

                await _userStore.SetUserNameAsync(gv, gv.Username, CancellationToken.None);
                var result = await _userManager.CreateAsync(gv, "Utc@123");

                if (result.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(gv);
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            return View();
            
        }

        public IActionResult Edit(string id)
        {
            if (id == null || db.GiaoVien == null)
            {
                return NotFound();
            }
            var gv = db.GiaoVien.Find(id);
            if (gv == null)
            {
                return NotFound();
            }
            ViewBag.MonHocID = new SelectList(db.MonHoc, "MonHocID", "TenMon", gv.MonHocID);
            return View(gv);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(string id, [Bind("Id","GiaoVienID", "HoVaTen", "Nu", "NgaySinh", "MonHocID", "GhiChu")] GiaoVien gv)
        {
            if (id != gv.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var user = db.GiaoVien.First(u => u.Id == gv.Id);
                    user.GiaoVienID = gv.GiaoVienID;
                    user.HoVaTen=gv.HoVaTen;
                    user.Nu=gv.Nu;
                    user.NgaySinh = gv.NgaySinh;
                    user.MonHocID=gv.MonHocID;
                    user.GhiChu = gv.GhiChu;
                    user.UserName = user.Username;
                    db.GiaoVien.Update(user);
                    db.SaveChanges();
                    await _userManager.UpdateAsync(user);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Gvexist(gv.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewBag.MonHocID = new SelectList(db.MonHoc, "MonHocID", "TenMon", gv.MonHocID);
            return View(gv);
        }
        private bool Gvexist(string id)
        {
            return (db.GiaoVien?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult Delete(string id)
        {
            if (id == null || db.GiaoVien == null)
            {
                return NotFound();
            }
            var gv = db.GiaoVien.Include(l => l.MonHoc).Include(e => e.ctgd).FirstOrDefault(m => m.Id == id);
            if (gv == null)
            {
                return NotFound();
            }
            if (gv.ctgd.Count() > 0)
            {
                return RedirectToAction("Index", new {alert = true});
            }
            return View(gv);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(string id)
        {
            if (db.GiaoVien == null)
            {
                return Problem("Khong con giao vien");
            }
            var gv = db.GiaoVien.Find(id);
            

            if (gv != null)
            {
                db.GiaoVien.Remove(gv);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
