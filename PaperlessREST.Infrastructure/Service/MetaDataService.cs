using Microsoft.EntityFrameworkCore;
using PaperlessRest.Application.DTOs;
using PaperlessREST.Domain.Entities;
using PaperlessREST.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessREST.Infrastructure.Service
{


    public class MetaDataService : IMetaDataService
    {
        private readonly IMetadataRepository _metadataRepository;


        public MetaDataService(IMetadataRepository metaDataRepository)
        {
            _metadataRepository = metaDataRepository;
        }

        public MetaDataDto CreateMetaData(CreateMetaDataCommand createCommand)
        {
            try
            {
                var metaData = new MetaData(createCommand.OwnerId, createCommand.Name, createCommand.FileExtension, createCommand.Author);
                _metadataRepository.Add(metaData);
                return new MetaDataDto(metaData.Id, metaData.OwnerId, metaData.Name, metaData.FileExtension, metaData.Author);
            }
            catch (Exception ex)
            {
                throw new ValidationException($"Failed to create metaData: {ex.Message}");
            }
        }

        public MetaDataDto? GetMetaDataByGuid(Guid guid)
        {
            var metaData = _metadataRepository.GetByGuid(guid);
            if (metaData == null)
                throw new Exception($"metaData with guid {guid} not found");

            return new MetaDataDto(metaData.Id, metaData.OwnerId, metaData.Name, metaData.FileExtension, metaData.Author);
        }

        public IEnumerable<MetaDataDto> GetAllMetaData()
        {
            var metaDatas = _metadataRepository.GetAll();

            return metaDatas
                .Select(m => new MetaDataDto(m.Id, m.OwnerId, m.Name, m.FileExtension, m.Author))
                .ToList();
        }
    }
}
