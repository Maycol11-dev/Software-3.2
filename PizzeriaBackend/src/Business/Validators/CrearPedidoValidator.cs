using FluentValidation;
using Models;

namespace Business.Validators;

public class CrearPedidoValidator : AbstractValidator<CrearPedidoRequest>
{
    public CrearPedidoValidator()
    {
        RuleFor(x => x.ClienteId)
            .GreaterThan(0).WithMessage("El cliente es obligatorio.");

        RuleFor(x => x.Pizzas)
            .NotEmpty().WithMessage("El pedido debe incluir al menos una pizza.");

        RuleForEach(x => x.Pizzas).ChildRules(pizza =>
        {
            pizza.RuleFor(x => x.IdPizza)
                .GreaterThan(0).WithMessage("El id de la pizza debe ser mayor a cero.");

            pizza.RuleFor(x => x.Cantidad)
                .GreaterThan(0).WithMessage("La cantidad de cada pizza debe ser mayor a cero.");
        });
    }
}
