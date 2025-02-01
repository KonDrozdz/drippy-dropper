namespace Drippy_Dropper.API.Models.Entities
{
    public class File
    {
        public Guid FileId { get; set; }
        public string Name { get; set; }
        public Guid FolderId { get; set; }
        public Guid OwnerId { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public string Path { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public Folder Folder { get; set; }
    }
}
