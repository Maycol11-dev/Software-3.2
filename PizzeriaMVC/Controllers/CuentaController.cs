using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Models;
using PizzeriaMVC.Services;

namespace PizzeriaMVC.Controllers;

public class CuentaController : Controller
{
    private readonly ApiPizzeria _api;

    public CuentaController(ApiPizzeria api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Registro() => View(new RegistroViewModel());

    [HttpPost]
    public async Task<IActionResult> Registro(RegistroViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        try
        {
            await _api.RegistrarAsync(new RegistroDto
            {
                Correo = modelo.Correo,
                Contrasena = modelo.Contrasena,
                Nombre = modelo.Nombre,
                Telefono = modelo.Telefono,
                Direccion = modelo.Direccion
            });

            var sesion = await _api.LoginAsync(modelo.Correo, modelo.Contrasena);
            if (sesion is null)
            {
                return RedirectToAction(nameof(Login));
            }

            await IniciarSesion(sesion);
            TempData["MensajeExito"] = $"¡Bienvenido, {sesion.Nombre}! Tu cuenta está lista.";
            return RedirectToAction(nameof(MenuController.Menu), "Menu");
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(modelo);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "No pudimos crear la cuenta. El servidor no está disponible.");
            return View(modelo);
        }
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) =>
        View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        try
        {
            var sesion = await _api.LoginAsync(modelo.Correo, modelo.Contrasena);
            if (sesion is null)
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
                return View(modelo);
            }

            await IniciarSesion(sesion);

            if (!string.IsNullOrWhiteSpace(modelo.ReturnUrl) && Url.IsLocalUrl(modelo.ReturnUrl))
            {
                return Redirect(modelo.ReturnUrl);
            }

            return RedirectToAction(nameof(MenuController.Menu), "Menu");
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(modelo);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "No pudimos iniciar sesión. El servidor no está disponible.");
            return View(modelo);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        _api.CerrarSesion();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    private async Task IniciarSesion(SesionDto sesion)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, sesion.UsuarioId.ToString()),
            new(ClaimTypes.Name, sesion.Nombre),
            new(ClaimTypes.Email, sesion.Correo),
            new("clienteId", sesion.ClienteId.ToString()),
            new("telefono", sesion.Telefono),
            new("direccion", sesion.Direccion),
            new(ClaimTypes.Role, sesion.RolId == 2 ? "Administrador" : "Cliente")
        };

        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidad));
    }
}
