using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoEfP26.Data.Entities
{
    public class User
    {
        public Guid      Id           { get; set; }
        public String    Name         { get; set; } = null!;
        public String    Email        { get; set; } = null!;
        public DateTime? Birthdate    { get; set; }
        public DateTime  RegisteredAt { get; set; }
        public DateTime? DeletedAt    { get; set; }

        // інверсна навігаційна властивість - властивість у іншій сутності по відношенню до даної
        public List<UserAccess> UserAccesses { get; set; } = [];   // додавання властивості -> доповнення конфігурації контексту
    }
}
