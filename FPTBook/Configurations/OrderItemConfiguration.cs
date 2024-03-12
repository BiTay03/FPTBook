using System;
using FPTBook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Identity.Client;
namespace FPTBook.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItem");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Order).WithMany(x => x.OrderItem).HasForeignKey(x => x.OrderId);
            builder.HasOne(x => x.Book).WithMany(x => x.OrderItems).HasForeignKey(x => x.BookId);

        }
    }
}


