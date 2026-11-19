using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PizzeriaMVC.Models;

public class PizzaFormViewModel
{
    public int IdPizza { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "La descripción no puede superar los 255 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0.01, 1000000, ErrorMessage = "El precio debe ser mayor a cero.")]
    public decimal Precio { get; set; }

    public string ImagenUrl { get; set; } = string.Empty;

    [Display(Name = "Imagen de la pizza")]
    public IFormFile? ArchivoImagen { get; set; }

    public bool EsEdicion => IdPizza > 0;
}
