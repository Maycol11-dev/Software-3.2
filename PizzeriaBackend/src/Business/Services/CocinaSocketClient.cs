using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Business.Services;

public class CocinaSocketClient : ICocinaGateway
{
    private const int Intentos = 2;

    private readonly string _host;
    private readonly int _puerto;

    public CocinaSocketClient(string host, int puerto)
    {
        _host = host;
        _puerto = puerto;
    }

    public async Task<string> PrepararPedidoAsync(int pedidoId)
    {
        Exception? ultimoError = null;

        for (var intento = 1; intento <= Intentos; intento++)
        {
            try
            {
                return await EnviarYEsperarAsync($"PREPARAR {pedidoId}");
            }
            catch (SocketException ex)
            {
                ultimoError = ex;
            }
            catch (IOException ex)
            {
                ultimoError = ex;
            }

            if (intento < Intentos)
            {
                await Task.Delay(500);
            }
        }

        throw new InvalidOperationException(
            $"La cocina no está disponible ({_host}:{_puerto}). Reintentamos {Intentos} veces sin éxito.",
            ultimoError);
    }

    private async Task<string> EnviarYEsperarAsync(string mensaje)
    {
        using var cliente = new TcpClient();

        await cliente.ConnectAsync(_host, _puerto);
        using var stream = cliente.GetStream();

        await WriteLineAsync(stream, mensaje);
        var respuesta = await ReadLineAsync(stream) ?? "ERROR sin respuesta de la cocina";

        if (respuesta.StartsWith("ERROR", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"La cocina rechazó el pedido: {respuesta}");
        }

        return respuesta;
    }

    private static async Task WriteLineAsync(NetworkStream stream, string mensaje)
    {
        var bytes = Encoding.UTF8.GetBytes(mensaje + "\n");
        await stream.WriteAsync(bytes);
    }

    private static async Task<string?> ReadLineAsync(NetworkStream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return await reader.ReadLineAsync();
    }
}