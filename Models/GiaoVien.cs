namespace btl_tkweb.Models
{
    public class GiaoVien
    {
        public GiaoVien() { }
        public int GiaoVienID { get; set; }
        public string HoVaTen { get; set; }
        public bool Nu { get; set; }
        public DateTime NgaySinh { get; set; }
        public int MonHocID { get; set; }
        public virtual MonHoc? MonHoc { get; set; }
        public string? GhiChu { get; set; }
        public virtual ICollection<ChiTietGiangDay> ctgd { get; set; }
    }
}
