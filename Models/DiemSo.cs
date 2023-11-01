namespace btl_tkweb.Models
{
    public class DiemSo
    {
        public DiemSo() { }
        public int DiemSoID { get; set; }
        public int MonHocID { get; set; }
        public int HocSinhId1 { get; set; }
        public MonHoc? MonHoc { get; set; }
        public HocSinh? HocSinh { get; set;}
        public float? DiemHeSo1_1 { get; set; }
        public float? DiemHeSo1_2 { get; set; }
        public float? DiemHeSo1_3 { get; set; }
        public float? DiemHeSo2_1 { get; set; }
        public float? DiemHeSo2_2 { get; set; }
        public float? DiemHeSo3 { get; set; }

    }
}
