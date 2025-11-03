using PaperlessREST.Domain.Common;

namespace PaperlessREST.Domain.Entities
{
    public class MetaData /*: BaseAuditableEntity*/
    {
        public MetaData(Guid id,string title, string fileType, double fileSize, string? summary, DateTime createdOn,DateTime modifiedLast)
        {
            this.Title = title;
            this.FileType = fileType;
            this.FileSize = fileSize;
            this.Summary = summary;
            this.Id = id;
            this.CreatedOn = createdOn;
            this.ModifiedLast = modifiedLast;

        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string FileType { get; set; }
        public double FileSize { get; set; }
        public string? Summary { get; set; }
        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedLast { get; set; }

    }
}