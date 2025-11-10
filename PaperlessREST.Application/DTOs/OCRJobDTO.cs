using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessREST.Application.DTOs
{
    public class OCRJobDTO
    {
        public string QueueMessage { get; set; }
        public string DocumentId { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        
        public OCRJobDTO(string QueueMessage, string DocumentId, string FileType, string FileName)
        {
            this.QueueMessage = QueueMessage;
            this.DocumentId = DocumentId;
            this.FileType = FileType;
            this.FileName = FileName;
        }
    }
}
