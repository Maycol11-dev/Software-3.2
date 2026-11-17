namespace PizzeriaMVC.Models;

public class PedidoPizzaDto
{
    public int IdPizza { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string NombrePizza { get; set; } = string.Empty;
}
