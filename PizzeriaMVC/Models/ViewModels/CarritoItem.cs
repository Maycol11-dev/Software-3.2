namespace PizzeriaMVC.Models;

public class CarritoItem
{
    public int IdPizza { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; } = 1;

    public decimal Subtotal => Precio * Cantidad;
}
