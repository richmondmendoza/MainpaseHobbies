using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.User
{
    public class AuthenticatedUserDto
    {
        public int Id { get; set; } = 0;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool HasAdminAccess { get; set; } = false;
        public bool HasCustomerAccess { get; set; } = false;
        public string AccessToken { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public List<string> Roles
        {
            get
            {
                var roles = Role.Split('|').Where(a => !string.IsNullOrEmpty(a));
                return roles.ToList();
            }
        }
    }
}
