namespace PizzeriaMVC.Models;

public class ConfirmarViewModel
{
    public CarritoViewModel Carrito { get; set; } = new();
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
}
