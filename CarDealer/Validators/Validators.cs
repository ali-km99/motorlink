using CarDealer.API.DTOs;
using CarDealer.API.DTOs.Car;
using FluentValidation;

namespace CarDealer.API.Validators;

// ─── Car ──────────────────────────────────────────────────────────────────────

public class CreateCarValidator : AbstractValidator<CreateCarDto>
{
    private static readonly string[] ValidCategories = { "Technology", "Interior", "Exterior" };
    private static readonly string[] ValidTransmissions = { "Automatic", "Manual", "CVT" };
    private static readonly string[] ValidConditions = { "New", "Used", "Like New" };
    private static readonly string[] ValidFuelTypes = { "Petrol", "Diesel", "Hybrid", "Electric" };
    private static readonly string[] ValidSpecs = { "Korean", "USA", "Gulf", "European", "Japanese" };
    private static readonly string[] ValidMileageUnits = { "KM", "MI" };
    private static readonly string[] ValidPaymentMethods = { "Cash", "Installment", "Both" };


public CreateCarValidator()
    {
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Year).InclusiveBetween(1990, DateTime.UtcNow.Year + 1);
        RuleFor(x => x.ExteriorColor).NotEmpty().MaximumLength(50);
        RuleFor(x => x.InteriorColor).MaximumLength(50);
        RuleFor(x => x.CostPrice).GreaterThan(0);
        RuleFor(x => x.ShippingCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SellingPrice).GreaterThan(0);
        RuleFor(x => x.StatusId).InclusiveBetween(1, 4);

        RuleFor(x => x.VinNumber).MaximumLength(17).MinimumLength(17)
            .When(x => !string.IsNullOrEmpty(x.VinNumber))
            .WithMessage("VIN number must be exactly 17 characters.");

        RuleFor(x => x.Mileage).GreaterThanOrEqualTo(0).When(x => x.Mileage.HasValue);
        RuleFor(x => x.MileageUnit).Must(v => ValidMileageUnits.Contains(v))
            .When(x => !string.IsNullOrEmpty(x.MileageUnit))
            .WithMessage("MileageUnit must be KM or MI.");

        RuleFor(x => x.NumberOfSeats).InclusiveBetween(1, 20).When(x => x.NumberOfSeats.HasValue);
        RuleFor(x => x.EngineSize).GreaterThan(0).When(x => x.EngineSize.HasValue);

        RuleFor(x => x.Transmission).Must(v => ValidTransmissions.Contains(v))
            .When(x => !string.IsNullOrEmpty(x.Transmission))
            .WithMessage("Transmission must be: Automatic, Manual, or CVT.");

        RuleFor(x => x.Condition).Must(v => ValidConditions.Contains(v))
            .When(x => !string.IsNullOrEmpty(x.Condition))
            .WithMessage("Condition must be: New, Used, or Like New.");

        RuleFor(x => x.FuelType).Must(v => ValidFuelTypes.Contains(v))
            .When(x => !string.IsNullOrEmpty(x.FuelType))
            .WithMessage("FuelType must be: Petrol, Diesel, Hybrid, or Electric.");

        RuleFor(x => x.Specs).Must(v => ValidSpecs.Contains(v))
            .When(x => !string.IsNullOrEmpty(x.Specs))
            .WithMessage("Specs must be: Korean, USA, Gulf, European, or Japanese.");

        RuleFor(x => x.PaymentMethod).Must(v => ValidPaymentMethods.Contains(v))
            .When(x => !string.IsNullOrEmpty(x.PaymentMethod))
            .WithMessage("PaymentMethod must be: Cash, Installment, or Both.");
    }


}

public class UpdateCarValidator : AbstractValidator<UpdateCarDto>
{
    private static readonly string[] ValidTransmissions = { "Automatic", "Manual", "CVT" };
    private static readonly string[] ValidConditions = { "New", "Used", "Like New" };
    private static readonly string[] ValidFuelTypes = { "Petrol", "Diesel", "Hybrid", "Electric" };
    private static readonly string[] ValidSpecs = { "Korean", "USA", "Gulf", "European", "Japanese" };
    private static readonly string[] ValidMileageUnits = { "KM", "MI" };
    private static readonly string[] ValidPaymentMethods = { "Cash", "Installment", "Both" };


public UpdateCarValidator()
    {
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Year).InclusiveBetween(1990, DateTime.UtcNow.Year + 1);
        RuleFor(x => x.ExteriorColor).NotEmpty().MaximumLength(50);
        RuleFor(x => x.CostPrice).GreaterThan(0);
        RuleFor(x => x.ShippingCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SellingPrice).GreaterThan(0);
        RuleFor(x => x.StatusId).InclusiveBetween(1, 4);

        RuleFor(x => x.VinNumber).MaximumLength(17).MinimumLength(17)
            .When(x => !string.IsNullOrEmpty(x.VinNumber))
            .WithMessage("VIN number must be exactly 17 characters.");

        RuleFor(x => x.Mileage).GreaterThanOrEqualTo(0).When(x => x.Mileage.HasValue);
        RuleFor(x => x.MileageUnit).Must(v => ValidMileageUnits.Contains(v))
            .When(x => !string.IsNullOrEmpty(x.MileageUnit));
        RuleFor(x => x.NumberOfSeats).InclusiveBetween(1, 20).When(x => x.NumberOfSeats.HasValue);
        RuleFor(x => x.EngineSize).GreaterThan(0).When(x => x.EngineSize.HasValue);
        RuleFor(x => x.Transmission).Must(v => ValidTransmissions.Contains(v))
            .When(x => !string.IsNullOrEmpty(x.Transmission));
        RuleFor(x => x.Condition).Must(v => ValidConditions.Contains(v))
            .When(x => !string.IsNullOrEmpty(x.Condition));
        RuleFor(x => x.FuelType).Must(v => ValidFuelTypes.Contains(v))
            .When(x => !string.IsNullOrEmpty(x.FuelType));
        RuleFor(x => x.Specs).Must(v => ValidSpecs.Contains(v))
            .When(x => !string.IsNullOrEmpty(x.Specs));
        RuleFor(x => x.PaymentMethod).Must(v => ValidPaymentMethods.Contains(v))
            .When(x => !string.IsNullOrEmpty(x.PaymentMethod));
    }


}

// ─── Sale ─────────────────────────────────────────────────────────────────────

public class CreateSaleValidator : AbstractValidator<CreateSaleDto>
{
    public CreateSaleValidator()
    {
        RuleFor(x => x.CarId)
        .GreaterThan(0).WithMessage("CarId is required.");


    RuleFor(x => x.CustomerId)
        .GreaterThan(0).WithMessage("CustomerId is required.");

        RuleFor(x => x.SoldPrice)
            .GreaterThan(0).WithMessage("Sold price must be greater than 0.");
    }


}

// ─── Customer ─────────────────────────────────────────────────────────────────

public class CreateCustomerValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Customer name is required.")
        .MaximumLength(150);

    RuleFor(x => x.Phone)
        .NotEmpty().WithMessage("Phone number is required.")
        .MaximumLength(50)
        .Matches(@"^[\d\s\+\-\(\)]+$").WithMessage("Phone number format is invalid.");
    }

}

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(50)
        .Matches(@"^[\d\s+-()]+$").WithMessage("Phone number format is invalid.");
    }
}

// ─── Feature ──────────────────────────────────────────────────────────────────

public class CreateFeatureValidator : AbstractValidator<CreateFeatureDto>
{
    public CreateFeatureValidator()
    {
        RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Feature name is required.")
        .MaximumLength(100).WithMessage("Feature name cannot exceed 100 characters.");
    }
}

public class UpdateFeatureValidator : AbstractValidator<UpdateFeatureDto>
{
    public UpdateFeatureValidator()
    {
        RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Feature name is required.")
        .MaximumLength(100);
    }
}
