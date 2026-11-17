using FluentValidation;
using Models;

namespace Business.Validators;

public class ClienteValidator : AbstractValidator<Cliente>
{
    public ClienteValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del cliente es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Direccion)
            .NotEmpty().WithMessage("La dirección del cliente es obligatoria.")
            .MaximumLength(150).WithMessage("La dirección no puede superar los 150 caracteres.");

        RuleFor(x => x.Telefono)
            .Must(TelefonoHelper.EsValido).WithMessage("El teléfono debe tener exactamente 10 dígitos.")
            .When(x => !string.IsNullOrWhiteSpace(x.Telefono));
    }
}
