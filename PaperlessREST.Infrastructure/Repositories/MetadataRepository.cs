using PaperlessRest.Application.DTOs;
using PaperlessREST.Domain.Entities;

namespace PaperlessREST.Infrastructure.Repositories
{
    public class MetadataRepository
    {

        private readonly PaperlessRestContext _context;

        public MetadataRepository(PaperlessRestContext context)
        {
            _context = context;
        }

        public async Task<List<MetaDataDto>> getAllBooks()
        {
            var list = new List<MetaDataDto>
            {
                new MetaDataDto
                {
                    Author = "Mehmet",
                    FileExtension = ".txt",
                    Name = "Teststoff",
                    OwnerId = 1,
                }
            };

            return await Task.FromResult(list);
        }

    }
}
