namespace PizzeriaMVC.Models;

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
