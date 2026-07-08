namespace AssetSquirrelAuthorize.WebApp.Components.ViewModels
{
    public class UserListItem
    {
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsLockedOut { get; set; }
    }
}
