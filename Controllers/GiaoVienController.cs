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
        private readonly UserManager<AccountUser> _userManager;
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public GiaoVienController(
            UserManager<AccountUser> userManager,
            SignInManager<AccountUser> signInManager,
            SchoolContext db
        )
        {
            this.db = db;
            _signInManager = signInManager;
            _userManager = userManager;
            GiaoVien.count = db.GiaoVien.Count();
        }
        private AccountUser getUser() { return _userManager.GetUserAsync(HttpContext.User).Result; }
        public IActionResult Index(bool? alert)
        {
            var user = getUser();
            if (user == null)return NotFound();

            if (alert == null) alert = false;
            ViewBag.Alert = alert;

            var gv = db.GiaoVien.Include(s => s.MonHoc).Include(g => g.ctgd).Where(g=>true);
            if (user.role == AccountUser.HOCSINH) gv = gv.Where(g => g.ctgd.Any(c => c.LopId == ((HocSinh)user).LopID));

            var lop = new List<Lop>();
            lop.Add(new Lop() { LopID = "" });
            if(user.role!=AccountUser.HOCSINH)lop.AddRange(db.Lop);
            ViewBag.lopId = new SelectList(lop, "LopID","LopID","");

            var mon= new List<MonHoc>();
            mon.Add(new MonHoc() { MonHocID= 0 ,TenMon="--",HeSoMon=0});
            mon.AddRange(db.MonHoc);
            ViewBag.MonHoc = new SelectList(mon, "MonHocID", "TenMon", "--");

            return View(gv);
        }
        public PartialViewResult Table(string LopID, int? MonHocID)
        {
            var user = getUser();

            if (LopID == "--") LopID = null;
            if(MonHocID==0)MonHocID=null;
            if (LopID == null && MonHocID == null)
            {
                var gv = db.GiaoVien.Include(s => s.MonHoc);
                if (user.role == AccountUser.HOCSINH) return PartialView(gv.Include(g => g.ctgd).Where(g => g.ctgd.Any(c => c.LopId == ((HocSinh)user).LopID)));
                return PartialView(gv);
            }
            else if (LopID == null)
            {
                var gv = db.GiaoVien.Include(s => s.MonHoc).Where(s => s.MonHocID == MonHocID).ToList();
                return PartialView(gv);
            }
            else if (MonHocID == null)
            {
                var gv = db.GiaoVien.Include(s => s.MonHoc).Include(s => s.ctgd).Where(s => s.ctgd.Any(c => c.LopId == LopID)).ToList();
                return PartialView(gv);
            }
            else
            {
                var gv = db.GiaoVien.Include(s => s.MonHoc).Include(s => s.ctgd).Where(s => s.ctgd.Any(c => c.LopId == LopID)).Where(s => s.MonHocID == MonHocID).ToList();
                return PartialView(gv);
            }
        }


        public IActionResult Create()
        {
            var user = getUser();
            if (user != null && user.role == AccountUser.ADMIN)
            {
                ViewBag.MonHocID = new SelectList(db.MonHoc, "MonHocID", "TenMon", "");
                return View();
            }
            return NotFound();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HoVaTen", "Nu","NgaySinh", "MonHocID","GhiChu")] GiaoVien gv)
        {
            var user = getUser();
            if (user != null && user.role == AccountUser.ADMIN)
            {

                if (ModelState.IsValid)
                {
                    gv.UserName = gv.Username;
                    var result = await _userManager.CreateAsync(gv, "Utc@123");

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                }
                return View();
            }
            return NotFound();
            
        }

        public IActionResult Edit(string id)
        {

            var user = getUser();
            if (user != null && ((user.role == AccountUser.ADMIN)||(user.role==AccountUser.GIAOVIEN && user.Id==id)))
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
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(string id, [Bind("Id","GiaoVienID", "HoVaTen", "Nu", "NgaySinh", "MonHocID", "GhiChu")] GiaoVien gv)
        {

            var user = getUser();
            if (user != null && ((user.role == AccountUser.ADMIN) || (user.role == AccountUser.GIAOVIEN && user.Id == id)))
            {
                if (id != gv.Id)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        var dbuser = db.GiaoVien.First(u => u.Id == gv.Id);
                        dbuser.GiaoVienID = gv.GiaoVienID;
                        dbuser.HoVaTen = gv.HoVaTen;
                        dbuser.Nu = gv.Nu;
                        dbuser.NgaySinh = gv.NgaySinh;
                        dbuser.MonHocID = gv.MonHocID;
                        dbuser.GhiChu = gv.GhiChu;
                        dbuser.UserName = dbuser.Username;
                        db.GiaoVien.Update(dbuser);
                        db.SaveChanges();
                        await _userManager.UpdateAsync(dbuser);
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
            return NotFound();
        }
        private bool Gvexist(string id)
        {
            return (db.GiaoVien?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult Delete(string id)
        {

            var user = getUser();
            if (user != null && user.role == AccountUser.ADMIN)
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
                    return RedirectToAction("Index", new { alert = true });
                }
                return View(gv);
            }
            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(string id)
        {

            var user = getUser();
            if (user != null && user.role == AccountUser.ADMIN)
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
            return NotFound();
        }

    }
}
