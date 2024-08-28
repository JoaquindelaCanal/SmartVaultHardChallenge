using System;

namespace SmartVault.Program.BusinessObjects
{
    public partial class Account
    {
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
