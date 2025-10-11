using Microsoft.EntityFrameworkCore;
using PaperlessREST.Application.DTOs;
using PaperlessREST.Application.Commands;
using PaperlessREST.Domain.Entities;
using PaperlessREST.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaperlessREST.DataAccess.Service;

namespace PaperlessREST.DataAccess.Service
{
    public class MetaDataService : IMetaDataService
    {
        private readonly IMetadataRepository _metadataRepository;


        public MetaDataService(IMetadataRepository metaDataRepository)
        {
            _metadataRepository = metaDataRepository;
        }

        public MetaData CreateMetaData(CreateMetaDataCommand createCommand)
        {
            try
            {
                var metaData = new MetaData(createCommand.Id,createCommand.Title, createCommand.FileType, createCommand.FileSize, createCommand.Summary,createCommand.CreatedOn,createCommand.ModifiedLast);
                _metadataRepository.Add(metaData);
                return new MetaData(metaData.Id, metaData.Title, metaData.FileType, metaData.FileSize, metaData.Summary,metaData.CreatedOn,metaData.ModifiedLast);
            }
            catch (Exception ex)
            {
                throw new ValidationException($"Failed to create metaData: {ex.Message}");
            }
        }

        public MetaData? GetMetaDataByGuid(Guid guid)
        {
            var metaData = _metadataRepository.GetByGuid(guid);
            if (metaData == null)
                throw new Exception($"metaData with guid {guid} not found");

            return new MetaData(metaData.Id, metaData.Title, metaData.FileType, metaData.FileSize, metaData.Summary, metaData.CreatedOn, metaData.ModifiedLast);
        }

        public IEnumerable<MetaData> GetAllMetaData()
        {
            var metaDatas = _metadataRepository.GetAll();

            return metaDatas.Select(m => new MetaData(m.Id, m.Title, m.FileType, m.FileSize, m.Summary, m.CreatedOn, m.ModifiedLast)).ToList();
        }

        public void DeleteMetadata(Guid guid)
        {
            var metaData = _metadataRepository.GetByGuid(guid);
            if (metaData!= null)
            {
                _metadataRepository.Delete(metaData);
                return;
            }
            throw new Exception($"metaData with guid {guid} not found");

        }

        public void UpdateMetadata(MetaData updatedMetaData)
        {
            /*var metaData = _metadataRepository.GetByGuid(updatedMetaData.Id);
            
            if (metaData == null)
                throw new Exception($"metaData with guid {updatedMetaData.Id} not found");*/

            _metadataRepository.Update(updatedMetaData);

        }
    }
}