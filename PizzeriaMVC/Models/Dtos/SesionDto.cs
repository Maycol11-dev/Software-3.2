namespace PizzeriaMVC.Models;

public class SesionDto
{
    public string Token { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public int ClienteId { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public int RolId { get; set; }
}
