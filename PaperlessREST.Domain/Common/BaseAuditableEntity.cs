using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessREST.Domain.Common
{
    public abstract class BaseAuditableEntity : BaseEntity
    {
        public DateTimeOffset Created { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTimeOffset LastModified { get; set; }

    }
}
