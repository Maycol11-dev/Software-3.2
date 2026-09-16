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
}