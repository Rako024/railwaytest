using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Core.Entities
{
    public class Category : BaseEntitiy
    {
        public string Name { get; set; }

        // Self-referencing navigation properties
        public int? SuperCategoryId { get; set; }
        public Category? SuperCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public List<Product>? Products { get; set; } = new List<Product>();
    }
}
