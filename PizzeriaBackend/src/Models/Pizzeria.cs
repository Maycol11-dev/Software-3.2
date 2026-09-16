namespace Models;

public class Pizzeria
{
    public int IdPizzeria { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string HorarioAtencion { get; set; } = string.Empty;
}