namespace DotNetTraining.Domains.Models
{
    public class UserModel
    {
        public Guid? UserId { get; set; }
        public string? FullName { get; set; }
    }

    public class UserEmailModel
    {
        public Guid? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
    }

    public class UserInformationModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string AzureUserId { get; set; } = string.Empty;
    }
}
