using Microsoft.EntityFrameworkCore;
using PaperlessREST.Application.DTOs;
using PaperlessREST.Domain.Entities;
using PaperlessREST.Infrastructure;
using PaperlessREST.Infrastructure.Repositories;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace PaperlessREST.Infrastructure.Repositories
{
    public class MetadataRepository : IMetadataRepository
    {

        private readonly PaperlessRestContext _context;

        public MetadataRepository(PaperlessRestContext context)
        {
            _context = context;
        }

        public IQueryable<MetaData> GetAll() => _context.MetaDatas.AsQueryable();

        public MetaData? GetByGuid(Guid guid) => _context.MetaDatas.SingleOrDefault(m => m.Id == guid);

        public MetaData? GetByUser(Guid userId) => _context.MetaDatas.SingleOrDefault(m => m.OwnerId == userId);

        public void Add(MetaData metadata)
        {
            try
            {
                _context.MetaDatas.Add(metadata);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new ValidationException($"Failed to add Metadata to database: {ex.Message}");
            }

        }

        public void Update(MetaData metaData)
        {
            //TODO: https://www.learnentityframeworkcore.com/concurrency
            try
            {
                _context.MetaDatas.Update(metaData);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Not found");
                //  throw new EntityNotFoundException();
            }
        }

        public void Delete(MetaData metaData)
        {
            _context.MetaDatas.Remove(metaData);
            _context.SaveChanges();
        }
    }
}
