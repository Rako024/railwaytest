﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.DTOs.CategoryDtos
{
    public record UpdateCategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? SuperCategoryId { get; set; } // Üst kategoriya üçün
    }
}
