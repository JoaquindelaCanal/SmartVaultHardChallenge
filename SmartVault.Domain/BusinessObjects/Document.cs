using System;

namespace SmartVault.Domain.BusinessObjects
{
    public partial class Document
    {
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
