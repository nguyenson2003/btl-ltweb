using btl_tkweb.Data;
using btl_tkweb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace btl_tkweb.ViewComponents
{
    public class LeftMenuViewComponent : ViewComponent
    {
        SchoolContext _context;
        UserManager<AccountUser> UserManager;
        private List<MenuItem> MenuItems = new List<MenuItem>();
        public LeftMenuViewComponent(
            SchoolContext context, 
            UserManager<AccountUser> UserManager
        )
        {
            _context = context;
            this.UserManager = UserManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            MenuItems.Add(new MenuItem() { Name = "Home" ,controllerName="Home",actionName="Index"});
            if(user != null )
            {
                MenuItems.Add(new MenuItem() { Name = "Danh sách lớp" ,controllerName="Lop",actionName="Index"});
                MenuItems.Add(new MenuItem() { Name = "Môn học" ,controllerName="MonHoc",actionName="Index"});
                MenuItems.Add(new MenuItem() { Name = "Danh sách giáo viên" ,controllerName="GiaoVien",actionName="Index"});
                if (user.role==AccountUser.ADMIN)
                    MenuItems.Add(new MenuItem() { Name = "Danh sách học sinh", controllerName = "HocSinh", actionName = "Index" });

            }

            return View("DefaultLeftMenu", MenuItems);
        }
    }
}
 