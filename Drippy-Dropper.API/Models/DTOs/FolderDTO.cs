using Drippy_Dropper.API.Models.Entities;

namespace Drippy_Dropper.API.Models.DTOs
{
    public class FolderDTO
    {
        public Guid FolderId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public Guid? ParentFolderId { get; set; }
        public int FileCount { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
