using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.DTOs.WishlistDtos
{
    public record WishlistDto
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public IEnumerable<WishlistItemDto> Items { get; set; }
    }
}
