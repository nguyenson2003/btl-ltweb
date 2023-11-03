using btl_tkweb.Data;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.AspNetCore.Identity;
using btl_tkweb.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SchoolContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolContext")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<AccountUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    
})
    .AddEntityFrameworkStores<SchoolContext>();

builder.Services.AddIdentityCore<AccountUser>()
    .AddEntityFrameworkStores<SchoolContext>();
builder.Services.AddIdentityCore<HocSinh>()
    .AddEntityFrameworkStores<SchoolContext>();
builder.Services.AddIdentityCore<GiaoVien>()
    .AddEntityFrameworkStores<SchoolContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider; 
    DbInit.Init(services);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/Identity/Account/Register", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true, true)));
    endpoints.MapPost("/Identity/Account/Register", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true, true)));
});
app.MapRazorPages();
app.Run();
