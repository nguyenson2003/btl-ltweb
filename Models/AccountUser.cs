using Microsoft.AspNetCore.Identity;

namespace btl_tkweb.Models
{
    public class AccountUser :IdentityUser
    {
        public AccountUser() { role = 0; }
        public int role { get; set; }
        public static readonly int ADMIN = 0;
        public static readonly int GIAOVIEN = 1;
        public static readonly int HOCSINH = 2;

    }
}
