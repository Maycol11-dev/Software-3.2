namespace PizzeriaMVC.Models;

public class EstadoPedidoStep
{
    public string Nombre { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public bool Completo { get; set; }
}
