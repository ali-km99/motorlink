using CarDealer.API.Features.Cars.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Cars;

public class CarStatusConfiguration : IEntityTypeConfiguration<CarStatus>
{
    public void Configure(EntityTypeBuilder<CarStatus> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Name).IsRequired();

        e.HasData(
            new CarStatus { Id = 1, Name = "Ready" },
            new CarStatus { Id = 2, Name = "Maintenance" },
            new CarStatus { Id = 3, Name = "Shipping" },
            new CarStatus { Id = 4, Name = "Sold" }
        );
    }
}