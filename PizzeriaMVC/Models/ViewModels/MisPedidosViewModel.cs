namespace PizzeriaMVC.Models;

public class MisPedidosViewModel
{
    public List<PedidoHistorial> Pedidos { get; set; } = new();
    public string NombreCliente { get; set; } = string.Empty;
}
