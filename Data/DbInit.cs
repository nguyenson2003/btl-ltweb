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
                var lop = new Lop[] { new Lop { LopID = "10A1", GVCN = "Nguyen Thi A" } };
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
                foreach (var hs in hocsinh)
                {
                    foreach (var mon in monhoc)
                    {
                        DiemSo x = new DiemSo() { HocSinhId = hs.HocSinhID, MonHocID=mon.MonHocID};
                        context.DiemSo.Add(x);
                    }
                }
                context.SaveChanges();
                var giaovien = new GiaoVien[]
                {
                    new GiaoVien(){HoVaTen="Nguyễn Thị A", Nu=true,NgaySinh = DateTime.Parse("1990-01-01"),MonHocID=1}
                };
                foreach (var x in giaovien)
                {
                    context.GiaoVien.Add(x);
                }
                context.SaveChanges();
            }
        }
    }
}
