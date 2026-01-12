using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessREST.Test
{
    public class FakeMinioClient : IMinioClient
    {
        public Task PutObjectAsync(PutObjectArgs args, CancellationToken ct = default)
            => Task.CompletedTask;
    }
}
