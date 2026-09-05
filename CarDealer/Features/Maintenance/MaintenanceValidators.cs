using CarDealer.API.Features.Maintenance.DTOs;
using FluentValidation;

namespace CarDealer.API.Features.Maintenance;

public class CreateMaintenanceValidator : AbstractValidator<CreateMaintenanceDto>
{
    public CreateMaintenanceValidator()
    {
        RuleFor(x => x.CarId)
            .GreaterThan(0).WithMessage("CarId is required.");

        RuleFor(x => x.MaintenanceCenterId)
            .GreaterThan(0).WithMessage("MaintenanceCenterId is required.");

        RuleFor(x => x.IssueDescription)
            .NotEmpty().WithMessage("Issue description is required.")
            .MaximumLength(500);

        RuleFor(x => x.RepairCost)
            .GreaterThan(0).WithMessage("Repair cost must be greater than 0.");

        When(x => x.InitialPaidAmount.HasValue, () =>
        {
            RuleFor(x => x.InitialPaidAmount!.Value)
                .GreaterThan(0).WithMessage("Initial paid amount must be greater than 0.")
                .LessThanOrEqualTo(x => x.RepairCost)
                .WithMessage("Initial paid amount cannot exceed repair cost.");
        });

        RuleFor(x => x.PaymentNotes).MaximumLength(500);
    }
}

public class UpdateMaintenanceValidator : AbstractValidator<UpdateMaintenanceDto>
{
    public UpdateMaintenanceValidator()
    {
        RuleFor(x => x.IssueDescription)
            .NotEmpty().WithMessage("Issue description is required.")
            .MaximumLength(500);

        RuleFor(x => x.RepairCost)
            .GreaterThan(0).WithMessage("Repair cost must be greater than 0.");

        When(x => x.MaintenanceCenterId.HasValue, () =>
        {
            RuleFor(x => x.MaintenanceCenterId!.Value)
                .GreaterThan(0).WithMessage("MaintenanceCenterId is required.");
        });
    }
}

public class CreateMaintenancePaymentValidator : AbstractValidator<CreateMaintenancePaymentDto>
{
    public CreateMaintenancePaymentValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Payment amount must be greater than 0.");

        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class CreateMaintenanceCenterValidator : AbstractValidator<CreateMaintenanceCenterDto>
{
    public CreateMaintenanceCenterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Center name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Notes).MaximumLength(500);

        RuleForEach(x => x.Phones).SetValidator(new CreateMaintenanceCenterPhoneValidator());
    }
}

public class UpdateMaintenanceCenterValidator : AbstractValidator<UpdateMaintenanceCenterDto>
{
    public UpdateMaintenanceCenterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Center name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Notes).MaximumLength(500);

        RuleForEach(x => x.Phones).SetValidator(new CreateMaintenanceCenterPhoneValidator());
    }
}

public class CreateMaintenanceCenterPhoneValidator : AbstractValidator<CreateMaintenanceCenterPhoneDto>
{
    public CreateMaintenanceCenterPhoneValidator()
    {
        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("Phone label is required.")
            .MaximumLength(50);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(50)
            .Matches(@"^[\d\s\+\-\(\)]+$").WithMessage("Phone number format is invalid.");
    }
}