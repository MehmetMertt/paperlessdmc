using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PaperlessREST.Application.Commands
{
    public record CreateMetaDataCommand( // specifies the format of the query matching to the table
        Guid Id,
        [StringLength(400, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 400 characters")]
        string Title,
        [StringLength(200,MinimumLength = 0,ErrorMessage ="Filetype must be between 2 and 20 characters")]
        string FileType,
        double FileSize,
        string? Summary,
        DateTime CreatedOn,
        DateTime ModifiedLast,
        string ObjectName
    );
}