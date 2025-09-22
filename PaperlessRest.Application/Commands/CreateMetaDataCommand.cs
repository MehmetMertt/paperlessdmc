using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PaperlessREST.Infrastructure.Service
{
    public record CreateMetaDataCommand(
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
        string Name,
        Guid OwnerId,
        [StringLength(20,MinimumLength =2,ErrorMessage ="File exntension must be between 2 and 20 characters")]
        string FileExtension,
        string? Author
    );
}