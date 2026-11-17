namespace PizzeriaMVC.Models;

public class CarritoViewModel
{
    public List<CarritoItem> Items { get; set; } = new();

    public decimal Total => Items.Sum(i => i.Subtotal);

    public bool Vacio => Items.Count == 0;
}
