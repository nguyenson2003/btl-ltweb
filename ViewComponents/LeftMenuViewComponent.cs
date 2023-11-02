using btl_tkweb.Data;
using btl_tkweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SQLitePCL;

namespace btl_tkweb.ViewComponents
{
    public class LeftMenuViewComponent : ViewComponent
    {
        SchoolContext _context;
        private List<MenuItem> MenuItems = new List<MenuItem>();
        public LeftMenuViewComponent(SchoolContext context)
        {
            _context = context;

           
            
        }

        

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("DefaultLeftMenu", MenuItems);
        }
    }
}
 