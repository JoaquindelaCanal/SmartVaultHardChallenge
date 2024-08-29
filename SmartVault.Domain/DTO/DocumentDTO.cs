using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVault.Domain.DTO
{
    public class DocumentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public int Length { get; set; }
        public int AccountId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
