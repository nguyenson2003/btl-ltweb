namespace btl_tkweb.Models
{
    public class ChiTietGiangDay
    {
        public ChiTietGiangDay() 
        {
            
        }
        public int ChiTietGiangDayId{ get; set; }
        public string LopId { get; set; }
        public string GiaoVienID { get; set; }
        public Lop? Lop { get; set; }
        public GiaoVien? GiaoVien { get; set; }
    }
}
