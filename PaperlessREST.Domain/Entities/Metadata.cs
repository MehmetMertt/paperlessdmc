using PaperlessREST.Domain.Common;

namespace PaperlessREST.Domain.Entities
{
    public class MetaData : BaseAuditableEntity
    {

        public int OwnerId { get; set; }

        public string Name { get; set; }
        
        public string FileExtension { get; set; }

        public string? Author { get; set; }


    }
}
