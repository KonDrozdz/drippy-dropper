namespace Drippy_Dropper.API.Models.DTOs
{
    public class FileDTO
    {
        public Guid FileId { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public string Path { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
