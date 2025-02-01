namespace Drippy_Dropper.API.Models.Entities
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Folder>? Folders { get; set; }
        public ICollection<File>? Files { get; set; }
    }
}
