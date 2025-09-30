using PaperlessREST.Domain.Common;

namespace PaperlessREST.Domain.Entities
{
    public class MetaData : BaseAuditableEntity
    {
        public MetaData(string title, string fileType, int fileSize, string? summary)
        {
            Title = title;
            FileType = fileType;
            FileSize = fileSize;
            Summary = summary;
        }

        public string Title { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public string? Summary { get; set; }
    }
}
