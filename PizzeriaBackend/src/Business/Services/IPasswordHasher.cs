namespace Business.Services;

public interface IPasswordHasher
{
    string Hash(string contrasena);
    bool Verificar(string contrasena, string hash);
}