using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessREST.DataAccess
{
    public interface ITesseractService
    {
        string ExtractTextFromImage(Stream imageStream, string language = "eng");

    }
}
