using Business.Validators;
using Models;

namespace Tests;

public class ValidatorTests
{
    private readonly RegistroUsuarioValidator _registroValidator = new();
    private readonly ClienteValidator _clienteValidator = new();
    private readonly PizzaValidator _pizzaValidator = new();

    private static RegistroUsuario Registro(
        string? telefono = null,
        string? correo = null,
        string? contrasena = null) => new()
    {
        Correo = correo ?? "juan@correo.com",
        Contrasena = contrasena ?? "secreto123",
        Nombre = "Juan",
        Telefono = telefono ?? "1123456789",
        Direccion = "Calle 1"
    };

    [Fact]
    public void Registro_TelefonoNueveDigitos_Falla()
    {
        var resultado = _registroValidator.Validate(Registro(telefono: "112345678"));

        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, e => e.PropertyName == "Telefono");
    }

    [Fact]
    public void Registro_TelefonoDiezDigitos_ConGuiones_Pasa()
    {
        Assert.True(_registroValidator.Validate(Registro(telefono: "11-2345-6789")).IsValid);
    }

    [Fact]
    public void Registro_CorreoInvalido_Falla()
    {
        Assert.False(_registroValidator.Validate(Registro(correo: "no-es-un-correo")).IsValid);
    }

    [Fact]
    public void Registro_ContrasenaCorta_Falla()
    {
        Assert.False(_registroValidator.Validate(Registro(contrasena: "abc")).IsValid);
    }

    [Fact]
    public void Registro_DatosValidos_Pasa()
    {
        Assert.True(_registroValidator.Validate(Registro()).IsValid);
    }

    [Fact]
    public void Cliente_SinNombre_Falla()
    {
        var cliente = new Cliente { Nombre = "", Direccion = "Calle 1" };

        Assert.False(_clienteValidator.Validate(cliente).IsValid);
    }

    [Fact]
    public void Pizza_PrecioCero_Falla()
    {
        var pizza = new Pizza { Nombre = "Muzzarella", Precio = 0 };

        Assert.False(_pizzaValidator.Validate(pizza).IsValid);
    }
}
