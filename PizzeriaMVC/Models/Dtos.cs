namespace PizzeriaMVC.Models;

public class ClienteDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
}

public class CrearPedidoDto
{
    public int ClienteId { get; set; }
    public List<PedidoPizzaDto> Pizzas { get; set; } = new();
}

public class PedidoPizzaDto
{
    public int IdPizza { get; set; }
    public int Cantidad { get; set; }
}

public class PedidoDetalleDto
{
    public int IdPedido { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string DireccionEntrega { get; set; } = string.Empty;
    public List<PedidoPizzaDto> Pizzas { get; set; } = new();
}
