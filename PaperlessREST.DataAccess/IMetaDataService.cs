using PaperlessREST.Application.DTOs;
using PaperlessREST.Application.Commands;

namespace PaperlessREST.DataAccess.Service
{
    public interface IMetaDataService
    {
        MetaDataDto CreateMetaData(CreateMetaDataCommand createCommand);
        IEnumerable<MetaDataDto> GetAllMetaData();
        MetaDataDto? GetMetaDataByGuid(Guid guid);
    }
}