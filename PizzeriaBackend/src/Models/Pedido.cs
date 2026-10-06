namespace Models;

public enum PedidoEstado
{
    EsperaDeConfirmacion,
    EnPreparacion,
    EnViaje,
    Entregado
}

public class Pedido
{
    public int IdPedido { get; set; }
    public int IdCliente { get; set; }
    public List<PedidoPizza> Pizzas { get; set; } = new();
    public PedidoEstado Estado { get; set; } = PedidoEstado.EsperaDeConfirmacion;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string DireccionEntrega { get; set; } = string.Empty;
}