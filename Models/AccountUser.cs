﻿using Microsoft.AspNetCore.Identity;

namespace btl_tkweb.Models
{
    public class AccountUser :IdentityUser
    {
        public HocSinh HocSinh { get; set; }
    }
}