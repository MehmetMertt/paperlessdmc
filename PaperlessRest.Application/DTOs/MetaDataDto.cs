namespace PaperlessREST.Application.DTOs
{
    public class MetaDataDto
    {
        public MetaDataDto(Guid id, string title, string fileType, int fileSize, string? summary)
        {
            Id = id;
            Title = title;
            FileType = fileType;
            FileSize = fileSize;
            Summary = summary;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public string? Summary { get; set; }
    }
}