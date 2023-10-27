using System.ComponentModel.DataAnnotations;

namespace btl_tkweb.Models
{
    public class Lop
    {
        public Lop() {
            dshs = new HashSet<HocSinh>();
        }
        [Required(AllowEmptyStrings =false,ErrorMessage ="Chưa nhập tên lớp")]
        public string LopID { get; set; }
        [Required(AllowEmptyStrings =false,ErrorMessage ="Chưa nhập tên gvcn")]
        public string GVCN{ get; set; }
        public virtual ICollection<HocSinh> dshs { get; set; }
        public virtual ICollection<ChiTietGiangDay> ctgd { get; set; }
    }
}
