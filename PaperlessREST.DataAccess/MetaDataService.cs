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

        public MetaDataDto CreateMetaData(CreateMetaDataCommand createCommand)
        {
            try
            {
                var metaData = new MetaData(createCommand.Title, createCommand.FileType, createCommand.FileSize, createCommand.Summary);
                _metadataRepository.Add(metaData);
                return new MetaDataDto(metaData.Id, metaData.Title, metaData.FileType, metaData.FileSize, metaData.Summary);
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

            return new MetaDataDto(metaData.Id, metaData.Title, metaData.FileType, metaData.FileSize, metaData.Summary);
        }

        public IEnumerable<MetaDataDto> GetAllMetaData()
        {
            var metaDatas = _metadataRepository.GetAll();

            return metaDatas.Select(m => new MetaDataDto(m.Id, m.Title, m.FileType, m.FileSize, m.Summary)).ToList();
        }
    }
}