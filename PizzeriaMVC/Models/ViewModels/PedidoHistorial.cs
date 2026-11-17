namespace PizzeriaMVC.Models;

public class PedidoHistorial
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string DireccionEntrega { get; set; } = string.Empty;
    public List<PedidoHistorialItem> Items { get; set; } = new();
}
