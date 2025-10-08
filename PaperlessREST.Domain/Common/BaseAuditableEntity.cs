using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessREST.Domain.Common
{
    public abstract class BaseAuditableEntity /*: BaseEntity*/
    {
        private DateTimeOffset _created;
        private DateTimeOffset _lastModified;

        protected BaseAuditableEntity()
        {
            _created = DateTimeOffset.UtcNow;
            _lastModified = DateTimeOffset.UtcNow;
        }

        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        public Guid? CreatedBy { get; set; }

        public DateTimeOffset LastModified
        {
            get => _lastModified;
            set => _lastModified = value.ToUniversalTime();
        }
    }
}