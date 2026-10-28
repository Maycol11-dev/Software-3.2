namespace Models;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public int RolId { get; set; }
    public DateTime FechaRegistro { get; set; }
}