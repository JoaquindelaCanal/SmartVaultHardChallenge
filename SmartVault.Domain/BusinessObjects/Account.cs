using System;

namespace SmartVault.Domain.BusinessObjects
{
    public partial class Account
    {
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
