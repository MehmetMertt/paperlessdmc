using PaperlessREST.Application.Commands;
using PaperlessREST.Application.DTOs;
using PaperlessREST.Domain.Entities;

namespace PaperlessREST.DataAccess.Service
{
    public interface IMetaDataService
    {
        MetaData CreateMetaData(CreateMetaDataCommand createCommand);
        IEnumerable<MetaData> GetAllMetaData();
        MetaData? GetMetaDataByGuid(Guid guid);
        MetaData? GetMetaDataBySearch(string searchterm);
        public void DeleteMetadata(Guid guid);

        public void UpdateMetadata(MetaData updatedMetaData);



      
    }

}