using PaperlessREST.Domain.Common;

namespace PaperlessREST.Domain.Entities
{
    public class MetaData /*: BaseAuditableEntity*/
    {
        public MetaData(Guid id,string title, string fileType, double fileSize, string? summary, DateTime createdOn,DateTime modifiedLast, string objectName)
        {
            this.Title = title;
            this.FileType = fileType;
            this.FileSize = fileSize;
            this.Summary = summary;
            this.Id = id;
            this.CreatedOn = createdOn;
            this.ModifiedLast = modifiedLast;
            this.ObjectName = objectName;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string FileType { get; set; }
        public double FileSize { get; set; }
        public string? Summary { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedLast { get; set; }
        // public string? OcrText { get; set; } // for duplicate findings
        // public bool IsDuplicate { get; set; } // also for duplicate findings
        public string ObjectName { get; set; } // for minio
    }
}