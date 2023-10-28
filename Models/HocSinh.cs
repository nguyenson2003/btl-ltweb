namespace btl_tkweb.Models
{
    public class HocSinh
    {
        public HocSinh() {
            Password = "demo123456";
        }
        public int HocSinhID { get; set; }
        public string Ho { get; set; }
        public string Ten { get; set; }
        public bool Nu { get; set; }
        public DateTime NgaySinh { get; set; }
        public string LopID { get; set; }
        public virtual Lop? Lop { get; set; }
        public string? GhiChu { get; set; }
        public string Username { get { 
            string user = "";
            string HoVaTen = Ho + Ten;
            for (int i = 0; i < HoVaTen.Length; i++)
            {
                if (HoVaTen[i] == ' ') continue;
                user += HoVaTen[i];
            }
            user += HocSinhID + "@web.demo.io";
            return user;
        } }
        public string Password { get; set; }
        public virtual ICollection<DiemSo> DiemSo { get; set; }

    }
}
