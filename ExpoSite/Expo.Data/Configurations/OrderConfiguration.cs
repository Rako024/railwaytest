using Expo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            
                builder.Property(x => x.AppUserId).IsRequired();
                builder.Property(x => x.IsPaid).IsRequired();
                builder.Property(x => x.TotalPrice).IsRequired();


        }
    }
}

