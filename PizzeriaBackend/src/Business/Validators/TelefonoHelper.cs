using System.Text.RegularExpressions;

namespace Business.Validators;

public static class TelefonoHelper
{
    public static string Normalizar(string? telefono)
        => Regex.Replace(telefono ?? string.Empty, @"[^\d]", "");

    public static bool EsValido(string? telefono)
        => Normalizar(telefono).Length == 10;
}
