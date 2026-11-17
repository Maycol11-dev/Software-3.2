using System.ComponentModel.DataAnnotations;

namespace PizzeriaMVC.Models;

public class RegistroViewModel
{
    [Required(ErrorMessage = "Ingresá tu correo.")]
    [EmailAddress(ErrorMessage = "El correo no es válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresá una contraseña.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [StringLength(100, ErrorMessage = "La contraseña no puede superar los 100 caracteres.")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirmá tu contraseña.")]
    [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    public string ConfirmarContrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresá tu nombre.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresá tu teléfono.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "El teléfono debe tener exactamente 10 dígitos, sin espacios ni guiones.")]
    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresá tu dirección de entrega.")]
    [StringLength(150, ErrorMessage = "La dirección no puede superar los 150 caracteres.")]
    public string Direccion { get; set; } = string.Empty;
}
