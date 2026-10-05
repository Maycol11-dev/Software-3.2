using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Models;
using PizzeriaMVC.Services;

namespace PizzeriaMVC.Controllers;

public class SeguimientoController : Controller
{
    private readonly ApiPizzeria _api;

    public SeguimientoController(ApiPizzeria api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Seguimiento(int id, int paso = 1)
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

        var estados = new (string Nombre, string Emoji)[]
        {
            ("Espera de confirmación", "🕐"),
            ("En preparación", "👨‍🍳"),
            ("En viaje", "🛵"),
            ("Entregado", "🍕")
        };

        int pasoActual = paso;
        if (pedido is not null)
        {
            pasoActual = pedido.Estado switch
            {
                "EsperaDeConfirmacion" => 1,
                "EnPreparacion" => 2,
                "EnViaje" => 3,
                "Entregado" => 4,
                _ => paso
            };
        }

        var modelo = new SeguimientoViewModel
        {
            IdPedido = id,
            Total = pedido?.Total ?? 54700,
            DireccionEntrega = pedido?.DireccionEntrega ?? "Av. Siempre Viva 123",
            Pasos = estados
                .Select((e, i) => new EstadoPedidoStep
                {
                    Nombre = e.Nombre,
                    Emoji = e.Emoji,
                    Completo = i + 1 < pasoActual,
                    Activo = i + 1 == pasoActual
                })
                .ToList()
        };

        return View(modelo);
    }
}
