using PaperlessRest.Application.DTOs;

namespace PaperlessREST.Infrastructure.Service
{
    public interface IMetaDataService
    {
        MetaDataDto CreateMetaData(CreateMetaDataCommand createCommand);
        IEnumerable<MetaDataDto> GetAllMetaData();
        MetaDataDto? GetMetaDataByGuid(Guid guid);
    }
}