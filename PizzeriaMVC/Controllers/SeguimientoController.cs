using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Models;
using PizzeriaMVC.Services;

namespace PizzeriaMVC.Controllers;

[Authorize]
public class SeguimientoController : Controller
{
    private readonly ApiPizzeria _api;

    public SeguimientoController(ApiPizzeria api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Seguimiento(int id)
    {
        PedidoDetalleDto? pedido;
        try
        {
            pedido = await _api.GetPedidoAsync(id);
        }
        catch (HttpRequestException)
        {
            pedido = null;
        }

        if (pedido is null)
        {
            TempData["MensajeError"] = $"No se encontró el pedido #{id}.";
            return RedirectToAction(nameof(MisPedidosController.Index), "MisPedidos");
        }

        var estados = new (string Nombre, string Emoji)[]
        {
            ("Espera de confirmación", "🕐"),
            ("En preparación", "👨‍🍳"),
            ("En viaje", "🛵"),
            ("Entregado", "🍕")
        };

        int pasoActual = pedido.Estado switch
        {
            "EsperaDeConfirmacion" => 1,
            "EnPreparacion" => 2,
            "EnViaje" => 3,
            "Entregado" => 4,
            _ => 1
        };

        var modelo = new SeguimientoViewModel
        {
            IdPedido = id,
            Total = pedido.Total,
            DireccionEntrega = pedido.DireccionEntrega,
            Pasos = estados
                .Select((e, i) => new EstadoPedidoStep
                {
                    Nombre = e.Nombre,
                    Emoji = e.Emoji,
                    Completo = i + 1 < pasoActual,
                    Activo = i + 1 == pasoActual
                })
                .ToList(),
            Items = pedido.Pizzas.Select(i => new PedidoHistorialItem
            {
                IdPizza = i.IdPizza,
                Nombre = i.NombrePizza,
                Precio = i.PrecioUnitario,
                Cantidad = i.Cantidad
            }).ToList()
        };

        return View(modelo);
    }
}
