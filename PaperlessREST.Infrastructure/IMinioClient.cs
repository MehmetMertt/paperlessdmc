using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessREST.Infrastructure
{
    public interface IMinioClient
    {
        Task UploadAsync(string objectName, Stream data);
    }
}
