using System.Security.Cryptography;

namespace Business.Services;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iteraciones = 100_000;
    private static readonly HashAlgorithmName Algoritmo = HashAlgorithmName.SHA256;

    public string Hash(string contrasena)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(contrasena, salt, Iteraciones, Algoritmo, KeySize);
        return $"PBKDF2${Iteraciones}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
    }

    public bool Verificar(string contrasena, string hash)
    {
        var partes = hash.Split('$');
        if (partes.Length != 4 || partes[0] != "PBKDF2")
        {
            return false;
        }

        if (!int.TryParse(partes[1], out var iterations))
        {
            return false;
        }

        try
        {
            var salt = Convert.FromBase64String(partes[2]);
            var esperado = Convert.FromBase64String(partes[3]);
            var calculado = Rfc2898DeriveBytes.Pbkdf2(contrasena, salt, iterations, Algoritmo, esperado.Length);
            return CryptographicOperations.FixedTimeEquals(calculado, esperado);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
