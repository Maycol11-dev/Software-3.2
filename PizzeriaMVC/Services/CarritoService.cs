using PizzeriaMVC.Controllers;
using PizzeriaMVC.Models;

namespace PizzeriaMVC.Services;

public class CarritoService
{
    private readonly ApiPizzeria _api;

    public CarritoService(ApiPizzeria api)
    {
        _api = api;
    }

    public CarritoViewModel Obtener(ISession session)
        => session.Get<CarritoViewModel>(SessionKeys.Carrito) ?? new CarritoViewModel();

    public void Guardar(ISession session, CarritoViewModel carrito)
        => session.Set(SessionKeys.Carrito, carrito);

    public void Agregar(ISession session, int idPizza, int cantidad)
    {
        var carrito = Obtener(session);
        var existente = carrito.Items.FirstOrDefault(i => i.IdPizza == idPizza);

        if (existente is not null)
        {
            existente.Cantidad += cantidad;
        }
        else
        {
            carrito.Items.Add(new CarritoItem { IdPizza = idPizza, Cantidad = cantidad });
        }

        Guardar(session, carrito);
    }

    public void Sumar(ISession session, int idPizza, int delta)
    {
        var carrito = Obtener(session);
        var existente = carrito.Items.FirstOrDefault(i => i.IdPizza == idPizza);

        if (existente is null)
        {
            if (delta > 0)
            {
                carrito.Items.Add(new CarritoItem { IdPizza = idPizza, Cantidad = delta });
            }
        }
        else
        {
            existente.Cantidad += delta;
            if (existente.Cantidad <= 0)
            {
                carrito.Items.Remove(existente);
            }
        }

        Guardar(session, carrito);
    }

    public void Quitar(ISession session, int idPizza)
    {
        var carrito = Obtener(session);
        carrito.Items.RemoveAll(i => i.IdPizza == idPizza);
        Guardar(session, carrito);
    }

    public async Task<CarritoViewModel> ObtenerHidratado(ISession session)
    {
        var carrito = Obtener(session);
        await Hidratar(carrito);
        return carrito;
    }

    public async Task Hidratar(CarritoViewModel carrito)
    {
        if (carrito.Items.Count == 0)
        {
            return;
        }

        var pizzas = await _api.GetPizzasPorIdAsync(carrito.Items.Select(i => i.IdPizza));

        foreach (var item in carrito.Items)
        {
            if (pizzas.TryGetValue(item.IdPizza, out var pizza))
            {
                item.Nombre = pizza.Nombre;
                item.Precio = pizza.Precio;
            }
        }
    }
}