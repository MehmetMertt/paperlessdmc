using PaperlessREST.Domain.Common;

namespace PaperlessREST.Domain.Entities
{
    public class MetaData : BaseAuditableEntity
    {
        public MetaData(Guid ownerId, string name, string fileExtension, string? author)
        {
            OwnerId = ownerId;
            Name = name;
            FileExtension = fileExtension;
            Author = author;
        }


        public Guid OwnerId { get; set; }

        public string Name { get; set; }
        
        public string FileExtension { get; set; }

        public string? Author { get; set; }


    }
}
