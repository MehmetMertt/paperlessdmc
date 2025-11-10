using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessREST.OcrWorker
{
    public interface ITesseractService
    {
        public string ExtractTextFromImage(Stream imageStream, string language = "eng");
    }
}
