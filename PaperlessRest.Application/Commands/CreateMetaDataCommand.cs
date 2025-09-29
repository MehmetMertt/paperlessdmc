using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PaperlessREST.Application.Commands
{
    public record CreateMetaDataCommand( // specifies the format of the query matching to the table
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
        string Name,
        Guid OwnerId,
        [StringLength(20,MinimumLength = 2,ErrorMessage ="File exntension must be between 2 and 20 characters")]
        string FileExtension,
        string? Author
    );
}