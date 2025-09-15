using FluentValidation;

namespace EShopCase.Application.Features.CategoryFeature.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommandRequest>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategori adı zorunludur.")
            .Must(n => !string.IsNullOrWhiteSpace(n)).WithMessage("Kategori adı sadece boşluk olamaz.")
            .MaximumLength(100)
            .MinimumLength(3)
            .Must(n => n.Trim() == n).WithMessage("Kategori adının başında/sonunda boşluk olmamalıdır.");
    }
}