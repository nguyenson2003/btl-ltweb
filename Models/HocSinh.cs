﻿using btl_tkweb.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Text.RegularExpressions;

namespace btl_tkweb.Models
{
    public class HocSinh : AccountUser
    {
        public static int count=0;
        public HocSinh() {
            DiemSo=new HashSet<DiemSo>();
            role = HOCSINH;
            count++;
            HocSinhID = count;
        }
        public int HocSinhID { get; set; }
        public string Ho { get; set; }
        public string Ten { get; set; }
        public string HoVaTen { get { return Ho + " " + Ten; } }
        public bool Nu { get; set; }
        public DateTime NgaySinh { get; set; }
        public string LopID { get; set; }
        public virtual Lop? Lop { get; set; }
        public string? GhiChu { get; set; }
        public string Username { get { 
            string user = "HS";
            string HoVaTen = Ho + Ten;
            for (int i = 0; i < HoVaTen.Length; i++)
            {
                if (HoVaTen[i] == ' ') continue;
                user += HoVaTen[i];
            }
            user += HocSinhID;
            return removeUnicode.RemoveUnicode(user);
        } }
        public virtual ICollection<DiemSo> DiemSo { get; set; }

        public void createBangDiem(SchoolContext db)
        {
            foreach (var mon in db.MonHoc)
            {
                DiemSo d = new DiemSo() { MonHocID = mon.MonHocID, HocSinhId = this.Id };
                db.Add(d);
            }
            db.SaveChanges();
        }
        public void deleteBangDiem(SchoolContext db)
        {
            foreach (var d in db.DiemSo)
            {
                if(d.HocSinhId ==this.Id) { 
                    db.Remove(d);
                }
            }
            db.SaveChanges();
        }
    }
}
