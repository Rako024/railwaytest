﻿using Expo.Core.Entities;
using Expo.Data.DAL;
using Expo.Data.Repositories.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Data.Repositories.Concretes
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(ExpoContext expoContext) : base(expoContext)
        {
        }
    }
}