using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PaperlessREST.Application.Commands
{
    public record CreateMetaDataCommand( // specifies the format of the query matching to the table
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
        string Title,
        [StringLength(20,MinimumLength = 2,ErrorMessage ="Filetype must be between 2 and 20 characters")]
        string FileType,
        int FileSize,
        string? Summary
    );
}