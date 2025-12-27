namespace Infrastructure
{
    public class ActiveDirectoryUser
    {
        public ActiveDirectoryUser()
        {
            Email = string.Empty;
            UserName = string.Empty;
            DisplayName = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Sid = string.Empty;
        }

        public string Email { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sid { get; set; }
        public bool IsMapped { get; set; }
    }

}
