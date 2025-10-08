namespace PaperlessREST.Application.DTOs
{
    public class MetaDataDto
    {
        public MetaDataDto(Guid id, string title, string fileType, int fileSize, string? summary, DateTime createdOn, DateTime modifiedLast )
        {
            this.Id = id;
            this.Title = title;
            this.FileType = fileType;
            this.FileSize = fileSize;
            this.Summary = summary;
            this.CreatedOn = createdOn;
            this.ModifiedLast = modifiedLast;

        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public string? Summary { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedLast { get; set; }
    }
}