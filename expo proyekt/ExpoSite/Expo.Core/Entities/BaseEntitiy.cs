using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Core.Entities
{
    public class BaseEntitiy
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedDate{ get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
