using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordKey { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime InvalidLoginDate { get; set; }
        public int InvalidLoginCounter { get; set; }
        public string Role { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }
    }
}
