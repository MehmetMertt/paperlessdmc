using PaperlessREST.Domain.Entities;

namespace PaperlessREST.Infrastructure.Repositories
{
    public interface IMetadataRepository
    {
        void Add(MetaData metadata);
        void Delete(MetaData metaData);
        IQueryable<MetaData> GetAll();
        MetaData? GetByGuid(Guid guid);
        void Update(MetaData metaData);
    }
}