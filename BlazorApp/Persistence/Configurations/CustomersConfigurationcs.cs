namespace BlazorApp.Persistence.Configurations;

using BlazorApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);

        //builder.Property(c => c.Id)
        //    .IsRequired()
        //    .HasMaxLength(50);

        //builder.Property(c => c.CompanyName)
        //    .IsRequired()
        //    .HasMaxLength(100);

        //builder.Property(c => c.ContactName)
        //    .HasMaxLength(100);

        //builder.Property(c => c.Address).HasMaxLength(200);
        //builder.Property(c => c.City).HasMaxLength(50);
        //builder.Property(c => c.Region).HasMaxLength(50);
        //builder.Property(c => c.PostalCode).HasMaxLength(20);
        //builder.Property(c => c.Country).HasMaxLength(50);
        //builder.Property(c => c.Phone).HasMaxLength(30);
    }
}