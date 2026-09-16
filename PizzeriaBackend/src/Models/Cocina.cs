namespace Models;

public class Cocina
{
    public int IdCocina { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Disponible { get; set; }
    public int PedidosEnPreparacion { get; set; }
}