using System.ComponentModel.DataAnnotations;

namespace btl_tkweb.Models
{
    public class MonHoc
    {
        public MonHoc() { }
        public int MonHocID { get; set; }
        public string TenMon { get; set; }
        public int HeSoMon { get; set; }
        public virtual ICollection<DiemSo> DiemSo { get; set; }
        public virtual ICollection<GiaoVien> GiaoViens { get; set; }
    }
}
