using System.ComponentModel.DataAnnotations;

namespace PizzeriaMVC.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Ingresá tu correo.")]
    [EmailAddress(ErrorMessage = "El correo no es válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresá tu contraseña.")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
