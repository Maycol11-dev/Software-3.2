namespace PizzeriaMVC.Models;

public class PizzaMenuItem
{
    public int IdPizza { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}

public class CarritoItem
{
    public int IdPizza { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; } = 1;

    public decimal Subtotal => Precio * Cantidad;
}

public class CarritoViewModel
{
    public List<CarritoItem> Items { get; set; } = new();

    public decimal Total => Items.Sum(i => i.Subtotal);

    public bool Vacio => Items.Count == 0;
}

public class MenuViewModel
{
    public List<PizzaMenuItem> Pizzas { get; set; } = new();
    public CarritoViewModel Carrito { get; set; } = new();
}

public class EstadoPedidoStep
{
    public string Nombre { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public bool Completo { get; set; }
}

public class SeguimientoViewModel
{
    public int IdPedido { get; set; }
    public decimal Total { get; set; }
    public string DireccionEntrega { get; set; } = string.Empty;
    public List<EstadoPedidoStep> Pasos { get; set; } = new();
}

public class PedidoHistorialItem
{
    public int IdPizza { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }

    public decimal Subtotal => Precio * Cantidad;
}

public class PedidoHistorial
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string DireccionEntrega { get; set; } = string.Empty;
    public List<PedidoHistorialItem> Items { get; set; } = new();
}

public class MisPedidosViewModel
{
    public List<PedidoHistorial> Pedidos { get; set; } = new();
}
