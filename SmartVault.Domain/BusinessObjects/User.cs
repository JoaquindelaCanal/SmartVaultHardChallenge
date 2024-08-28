using System;

namespace SmartVault.Domain.BusinessObjects
{
    public partial class User
    {
        public string FullName => $"{FirstName} {LastName}";
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
