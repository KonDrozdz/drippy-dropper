namespace Drippy_Dropper.API.Models.Entities
{
    public class Folder
    {
        public Guid FolderId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public Guid? ParentFolderId { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public Folder? ParentFolder { get; set; }
        public ICollection<Folder>? SubFolders { get; set; }
        public ICollection<File>? Files { get; set; }
    }
}
