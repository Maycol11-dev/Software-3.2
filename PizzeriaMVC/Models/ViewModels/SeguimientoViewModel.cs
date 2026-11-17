namespace PizzeriaMVC.Models;

public class SeguimientoViewModel
{
    public int IdPedido { get; set; }
    public decimal Total { get; set; }
    public string DireccionEntrega { get; set; } = string.Empty;
    public List<EstadoPedidoStep> Pasos { get; set; } = new();
    public List<PedidoHistorialItem> Items { get; set; } = new();
}
