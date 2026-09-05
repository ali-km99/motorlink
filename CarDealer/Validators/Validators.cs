using CarDealer.API.Features.Cars.DTOs;
using CarDealer.API.Features.Customers.DTOs;
using CarDealer.API.Features.Sales.DTOs;
using FluentValidation;

namespace CarDealer.API.Validators;


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
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^(091|092|093|094)\d{7}$")
            .WithMessage("رقم الهاتف يجب أن يبدأ بـ 091 أو 092 أو 093 أو 094 ويكون مكون من 10 أرقام.");
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
