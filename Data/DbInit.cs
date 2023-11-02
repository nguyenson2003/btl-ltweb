using btl_tkweb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace btl_tkweb.Data
{
    public class DbInit
    {
        public static async void Init(IServiceProvider serviceProvider)
        {
            UserManager<AccountUser> userManager = serviceProvider.GetService<UserManager<AccountUser>>();
            
            using (var context = new SchoolContext(serviceProvider.GetRequiredService<DbContextOptions<SchoolContext>>()))
            {
                context.Database.EnsureCreated();
                if (context.AccountUser.Any())
                {
                    return;
                }

                //create admin account 
                AccountUser adminAcc = new AccountUser() { UserName = "Admin" };
                var adminResult = userManager.CreateAsync(adminAcc, "Utc@123").Result;
                if (!adminResult.Succeeded) throw new ArgumentException(adminAcc.UserName);

                //create Lop table
                var lop = new Lop[] {
                    new Lop { LopID = "10A1", GVCN = "Nguyen Thi A" },
                    new Lop { LopID = "10A2", GVCN = "Nguyen Thi X" },
                };
                foreach (var x in lop)
                {
                    context.Lop.Add(x);
                }
                context.SaveChanges();

                //create hoc sinh table
                HocSinh.count = 0;
                var hocsinh = new HocSinh[] { new HocSinh() { Ho = "Nguyen Văn", Ten = "B", NgaySinh = DateTime.Parse("2003-01-01"), LopID = "10A1", Nu = false } };
                foreach (var x in hocsinh)
                {
                    /*context.HocSinh.Add(x);
                    context.SaveChanges();*/
                    x.UserName = x.Username; 
                    var result = userManager.CreateAsync(x, "Utc@123").Result;
                    if (!result.Succeeded) throw new ArgumentException(x.UserName);
                }

                //create mon hoc table
                var monhoc = new MonHoc[] {
                    new MonHoc { TenMon="Toán",  HeSoMon=2},
                    new MonHoc { TenMon="Văn",  HeSoMon=2},
                    new MonHoc { TenMon="Anh",  HeSoMon=1},
                };
                foreach (var x in monhoc)
                {
                    context.MonHoc.Add(x);
                }
                context.SaveChanges();

                //create bang diem table
                foreach (var x in hocsinh)
                {
                    x.createBangDiem(context);
                }
                context.SaveChanges();

                //create giao vien table
                GiaoVien.count = 0;
                var giaovien = new GiaoVien[]
                {
                    new GiaoVien(){HoVaTen="Nguyễn Thị A", Nu=true,NgaySinh = DateTime.Parse("1990-01-01"),MonHocID=1},
                    new GiaoVien(){HoVaTen="Nguyễn Thị X", Nu=true,NgaySinh = DateTime.Parse("1990-01-01"),MonHocID=2},
                    new GiaoVien(){HoVaTen="Nguyễn Thị Y", Nu=true,NgaySinh = DateTime.Parse("1990-01-01"),MonHocID=3},
                    new GiaoVien(){HoVaTen="Nguyễn Thị Z", Nu=true,NgaySinh = DateTime.Parse("1990-01-01"),MonHocID=2},
                };
                foreach (var x in giaovien)
                {
                    /*context.GiaoVien.Add(x);*/
                    x.UserName = x.Username;
                    var result = userManager.CreateAsync(x, "Utc@123").Result;
                    if (!result.Succeeded) throw new ArgumentException(x.UserName);
                }
                context.SaveChanges();

                //create chi tiet giang day table
                var ctgd = new ChiTietGiangDay[]
                {
                    new ChiTietGiangDay(){LopHocId="10A1", GiaoVienID=giaovien[0].Id},
                    new ChiTietGiangDay(){LopHocId="10A1", GiaoVienID=giaovien[1].Id},
                    new ChiTietGiangDay(){LopHocId="10A1", GiaoVienID = giaovien[2].Id},
                    new ChiTietGiangDay(){LopHocId="10A2", GiaoVienID = giaovien[0].Id},
                    new ChiTietGiangDay(){LopHocId="10A2", GiaoVienID = giaovien[3].Id},
                };
                foreach (var x in ctgd)
                {
                    context.ChiTietGiangDay.Add(x);
                }
                context.SaveChanges();
            }
        }
    }
}
