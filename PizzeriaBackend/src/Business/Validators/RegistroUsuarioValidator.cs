using FluentValidation;
using Models;

namespace Business.Validators;

public class RegistroUsuarioValidator : AbstractValidator<RegistroUsuario>
{
    public RegistroUsuarioValidator()
    {
        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("El correo es obligatorio.")
            .EmailAddress().WithMessage("El correo no es válido.")
            .MaximumLength(150).WithMessage("El correo no puede superar los 150 caracteres.");

        RuleFor(x => x.Contrasena)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.")
            .MaximumLength(100).WithMessage("La contraseña no puede superar los 100 caracteres.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Telefono)
            .NotEmpty().WithMessage("El teléfono es obligatorio.")
            .Must(TelefonoHelper.EsValido).WithMessage("El teléfono debe tener exactamente 10 dígitos.");

        RuleFor(x => x.Direccion)
            .NotEmpty().WithMessage("La dirección es obligatoria.")
            .MaximumLength(150).WithMessage("La dirección no puede superar los 150 caracteres.");
    }
}
