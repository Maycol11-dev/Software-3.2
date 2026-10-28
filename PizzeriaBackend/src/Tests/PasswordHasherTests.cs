using Business.Services;
using Xunit;

namespace Tests;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Hash_GeneraHashConFormatoPBKDF2()
    {
        var hash = _hasher.Hash("123456");

        Assert.StartsWith("PBKDF2$", hash);
        var partes = hash.Split('$');
        Assert.Equal(4, partes.Length);
        Assert.Equal("PBKDF2", partes[0]);
        Assert.Equal(100_000, int.Parse(partes[1]));
    }

    [Fact]
    public void Verificar_CoincideConContrasenaCorrecta()
    {
        var hash = _hasher.Hash("pizza123");

        Assert.True(_hasher.Verificar("pizza123", hash));
        Assert.False(_hasher.Verificar("otra", hash));
    }

    [Fact]
    public void Verificar_HashMalformado_RetornaFalse()
    {
        Assert.False(_hasher.Verificar("123", "malformado"));
        Assert.False(_hasher.Verificar("123", "PBKDF2$abc$salt$hash"));
        Assert.False(_hasher.Verificar("123", "PBKDF2$100000$bad$salt"));
    }

    [Fact]
    public void Verificar_DiferenteContrasena_RetornaFalse()
    {
        var hash = _hasher.Hash("secreto");

        Assert.False(_hasher.Verificar("SECRETO", hash));
        Assert.False(_hasher.Verificar("secreta", hash));
    }
}
