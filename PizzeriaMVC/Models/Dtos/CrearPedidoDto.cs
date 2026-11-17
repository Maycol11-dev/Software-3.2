namespace PizzeriaMVC.Models;

public class CrearPedidoDto
{
    public int ClienteId { get; set; }
    public List<PedidoPizzaDto> Pizzas { get; set; } = new();
}
