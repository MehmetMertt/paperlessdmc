namespace PaperlessREST.Application.DTOs
{
    public class MetaDataDto
    {
        public MetaDataDto(Guid id, Guid ownerId, string name, string fileExtension, string? author)
        {
            Id = id;
            OwnerId = ownerId;
            Name = name;
            FileExtension = fileExtension;
            Author = author;
        }


        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string Name { get; set; }

        public string FileExtension { get; set; }

        public string? Author { get; set; }

    }
}
