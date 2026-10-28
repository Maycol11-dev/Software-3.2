using System.ComponentModel.DataAnnotations;

namespace PizzeriaMVC.Models;

public class PizzaMenuItem
{
    public int IdPizza { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}

public class CarritoItem
{
    public int IdPizza { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; } = 1;

    public decimal Subtotal => Precio * Cantidad;
}

public class CarritoViewModel
{
    public List<CarritoItem> Items { get; set; } = new();

    public decimal Total => Items.Sum(i => i.Subtotal);

    public bool Vacio => Items.Count == 0;
}

public class MenuViewModel
{
    public List<PizzaMenuItem> Pizzas { get; set; } = new();
    public CarritoViewModel Carrito { get; set; } = new();
}

public class EstadoPedidoStep
{
    public string Nombre { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public bool Completo { get; set; }
}

public class SeguimientoViewModel
{
    public int IdPedido { get; set; }
    public decimal Total { get; set; }
    public string DireccionEntrega { get; set; } = string.Empty;
    public List<EstadoPedidoStep> Pasos { get; set; } = new();
    public List<PedidoHistorialItem> Items { get; set; } = new();
}

public class PedidoHistorialItem
{
    public int IdPizza { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }

    public decimal Subtotal => Precio * Cantidad;
}

public class PedidoHistorial
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string DireccionEntrega { get; set; } = string.Empty;
    public List<PedidoHistorialItem> Items { get; set; } = new();
}

public class MisPedidosViewModel
{
    public List<PedidoHistorial> Pedidos { get; set; } = new();
    public string NombreCliente { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Ingresá tu correo.")]
    [EmailAddress(ErrorMessage = "El correo no es válido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresá tu contraseña.")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

public class RegistroViewModel
{
    [Required(ErrorMessage = "Ingresá tu correo.")]
    [EmailAddress(ErrorMessage = "El correo no es válido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresá una contraseña.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresá tu nombre.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresá tu teléfono.")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresá tu dirección de entrega.")]
    public string Direccion { get; set; } = string.Empty;
}

public class ConfirmarViewModel
{
    public CarritoViewModel Carrito { get; set; } = new();
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
}
