using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.User
{
    public class UserSessionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public System.DateTime Date { get; set; }
        public System.DateTime LastDateOfActivity { get; set; }
        public bool IsTerminated { get; set; }
        public string IPAddress { get; set; }
        public string FullName { get; set; }
    }
}
