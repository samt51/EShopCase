using FluentValidation;

namespace EShopCase.Application.Features.ProductsFeature.Commands.CreateProduct;

public class CreateProductCommandValidator :  AbstractValidator<CreateProductCommandRequest>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün adı zorunludur.")
            .MaximumLength(100)
            .MinimumLength(2);
    }
}