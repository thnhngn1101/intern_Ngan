namespace Common.Application.Models
{
    public class AuthenticatedUserModel
    {
        private static string _guestRole = "GUEST";
        public static string GuestRole
        {
            get => _guestRole;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _guestRole = value;
                }
            }
        }
        public string Role { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set;} = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
      

}
}
