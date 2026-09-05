using CarDealer.API.Features.Expenses.DTOs;
using FluentValidation;

namespace CarDealer.API.Validators;

public class CreateExpenseValidator : AbstractValidator<CreateExpenseDto>
{
    public CreateExpenseValidator()
    {
        RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("CategoryId is required.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than 0.");
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class UpdateExpenseValidator : AbstractValidator<UpdateExpenseDto>
{
    public UpdateExpenseValidator()
    {
        RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("CategoryId is required.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than 0.");
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class CreateExpenseCategoryValidator : AbstractValidator<CreateExpenseCategoryDto>
{
    public CreateExpenseCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100);
    }
}