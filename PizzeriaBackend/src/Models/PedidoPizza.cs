namespace Models;

public class PedidoPizza
{
    public int IdPedido { get; set; }
    public int IdPizza { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string NombrePizza { get; set; } = string.Empty;
    public decimal Subtotal => PrecioUnitario * Cantidad;
}