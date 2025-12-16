using Microsoft.EntityFrameworkCore;
using PaperlessREST.Application.DTOs;
using PaperlessREST.Domain.Entities;
using PaperlessREST.Infrastructure;
using PaperlessREST.Infrastructure.Repositories;
using System.ComponentModel.DataAnnotations;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public MetaData? GetBySearch(string searchterm)
        {
            if (string.IsNullOrWhiteSpace(searchterm))
                return null;

            searchterm = searchterm.ToLower();


            return (MetaData?)_context.MetaDatas.Where(m => m.Title.ToLower().Contains(searchterm));

        }

        public void Add(MetaData metadata)
        {
            try
            {
                _context.MetaDatas.Add(metadata);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new ValidationException($"Failed to add Metadata to database: {ex.Message}| Inner: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                // Fallback catch for unexpected exceptions
                throw new ValidationException(
                    $"Unexpected error while adding Metadata: {ex.Message} | Inner: {ex.InnerException?.Message}"
                );
            }

        }

        public void Update(MetaData metaData)
        {
            //TODO: https://www.learnentityframeworkcore.com/concurrency
            try
            {

                var existingMetaData=_context.MetaDatas.Find(metaData.Id);
                existingMetaData.Title = metaData.Title;
                existingMetaData.Summary = metaData.Summary;
                existingMetaData.FileType = metaData.FileType;
                existingMetaData.FileSize = metaData.FileSize;
                existingMetaData.ModifiedLast = DateTime.UtcNow;

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
