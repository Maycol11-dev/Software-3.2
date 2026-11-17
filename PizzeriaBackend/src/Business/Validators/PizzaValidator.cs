using FluentValidation;
using Models;

namespace Business.Validators;

public class PizzaValidator : AbstractValidator<Pizza>
{
    public PizzaValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la pizza es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(255).WithMessage("La descripción no puede superar los 255 caracteres.");

        RuleFor(x => x.Precio)
            .GreaterThan(0).WithMessage("El precio de la pizza debe ser mayor a cero.");

        RuleFor(x => x.ImagenUrl)
            .MaximumLength(255).WithMessage("La URL de la imagen no puede superar los 255 caracteres.");
    }
}
