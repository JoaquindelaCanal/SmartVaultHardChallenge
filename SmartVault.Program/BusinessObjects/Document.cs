using System;

namespace SmartVault.Program.BusinessObjects
{
    public partial class Document
    {
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
