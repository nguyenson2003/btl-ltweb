using Microsoft.EntityFrameworkCore.Design.Internal;
using System.ComponentModel.DataAnnotations;

namespace btl_tkweb.Models
{
    public class GiaoVien : AccountUser
    {
        public static int count=0;
        public GiaoVien()
        {
            ctgd = new HashSet<ChiTietGiangDay>();
            role = GIAOVIEN;
            count++;
            GiaoVienID = count;
        }
        
        public int GiaoVienID { get; set; }

        
        public string HoVaTen { get; set; }

        public bool Nu { get; set; }
        public DateTime NgaySinh { get; set; }
        public int MonHocID { get; set; }
        public virtual MonHoc? MonHoc { get; set; }
        public string? GhiChu { get; set; }
        public  string Username { get { 
                string user = "GV";
                HoVaTen= ""+HoVaTen;
                for (int i = 0; i < HoVaTen.Length; i++)
                {
                    if (HoVaTen[i] == ' ') continue;
                    user += HoVaTen[i];
                }
                user += GiaoVienID ;
                return removeUnicode.RemoveUnicode(user);
        } }
        public virtual ICollection<ChiTietGiangDay> ctgd { get; set; }
    }
}
