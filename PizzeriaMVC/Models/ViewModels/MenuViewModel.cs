namespace PizzeriaMVC.Models;

public class MenuViewModel
{
    public List<PizzaMenuItem> Pizzas { get; set; } = new();
    public CarritoViewModel Carrito { get; set; } = new();
}
