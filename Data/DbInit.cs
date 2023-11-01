using btl_tkweb.Models;
using Microsoft.EntityFrameworkCore;

namespace btl_tkweb.Data
{
    public class DbInit
    {
        public static void Init(IServiceProvider serviceProvider)
        {
            using(var context = new SchoolContext(serviceProvider.GetRequiredService<DbContextOptions<SchoolContext>>()))
            {
                context.Database.EnsureCreated();
                if (context.HocSinh.Any())
                {
                    return;
                }
                var lop = new Lop[] { 
                    new Lop { LopID = "10A1", GVCN = "Nguyen Thi A" },
                    new Lop { LopID = "10A2", GVCN = "Nguyen Thi X" },
                };
                foreach (var x in lop)
                {
                    context.Lop.Add(x);
                }
                var hocsinh = new HocSinh[] { new HocSinh { Ho = "Nguyen Văn", Ten = "B", NgaySinh = DateTime.Parse("2003-01-01"), LopID = "10A1", Nu = false} };
                foreach (var x in hocsinh)
                {
                    context.HocSinh.Add(x);
                }
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
                foreach (var x in hocsinh)
                {
                    x.createBangDiem(context);
                }
                context.SaveChanges();
                var giaovien = new GiaoVien[]
                {
                    new GiaoVien(){HoVaTen="Nguyễn Thị A", Nu=true,NgaySinh = DateTime.Parse("1990-01-01"),MonHocID=1},
                    new GiaoVien(){HoVaTen="Nguyễn Thị X", Nu=true,NgaySinh = DateTime.Parse("1990-01-01"),MonHocID=2},
                    new GiaoVien(){HoVaTen="Nguyễn Thị Y", Nu=true,NgaySinh = DateTime.Parse("1990-01-01"),MonHocID=3},
                    new GiaoVien(){HoVaTen="Nguyễn Thị Z", Nu=true,NgaySinh = DateTime.Parse("1990-01-01"),MonHocID=2},
                };
                foreach (var x in giaovien)
                {
                    context.GiaoVien.Add(x);
                }
                context.SaveChanges();
                var ctgd = new ChiTietGiangDay[]
                {
                    new ChiTietGiangDay(){LopHocId="10A1", GiaoVienID1=1},
                    new ChiTietGiangDay(){LopHocId="10A1", GiaoVienID1=2},
                    new ChiTietGiangDay(){LopHocId="10A1", GiaoVienID1=3},
                    new ChiTietGiangDay(){LopHocId="10A2", GiaoVienID1=1},
                    new ChiTietGiangDay(){LopHocId="10A2", GiaoVienID1=4},
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
