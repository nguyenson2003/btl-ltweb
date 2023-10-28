namespace btl_tkweb.Models
{
    public class GiaoVien
    {
        public GiaoVien()
        {
            Password = "demo123456";
        }
        public int GiaoVienID { get; set; }
        public string HoVaTen { get; set; }
        public bool Nu { get; set; }
        public DateTime NgaySinh { get; set; }
        public int MonHocID { get; set; }
        public virtual MonHoc? MonHoc { get; set; }
        public string? GhiChu { get; set; }
        public string Username { get { 
                string user = "";
                for (int i = 0; i < HoVaTen.Length; i++)
                {
                    if (HoVaTen[i] == ' ') continue;
                    user += HoVaTen[i];
                }
                user += GiaoVienID + "@web.demo.io";
                return Username;
        } }
        public string Password { get; set; }
        public virtual ICollection<ChiTietGiangDay> ctgd { get; set; }
    }
}
