namespace btl_tkweb.Models
{
    public class HocSinh
    {
        public HocSinh() { }
        public int HocSinhID { get; set; }
        public string Ho { get; set; }
        public string Ten { get; set; }
        public bool Nu { get; set; }
        public DateTime NgaySinh { get; set; }
        public string LopID { get; set; }
        public virtual Lop? Lop { get; set; }
        public string? GhiChu { get; set; }
        public virtual ICollection<DiemSo> DiemSo { get; set; }

    }
}
