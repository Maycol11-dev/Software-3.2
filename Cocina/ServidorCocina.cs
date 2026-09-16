using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Cocina;

public class ServidorCocina
{
    private readonly int _puerto;

    public ServidorCocina(int puerto)
    {
        _puerto = puerto;
    }

    public async Task IniciarAsync()
    {
        var listener = new TcpListener(IPAddress.Loopback, _puerto);
        listener.Start();
        Console.WriteLine($"Servidor de cocina escuchando en el puerto {_puerto}...");

        while (true)
        {
            var cliente = await listener.AcceptTcpClientAsync();
            _ = AtenderClienteAsync(cliente);
        }
    }

    private static async Task AtenderClienteAsync(TcpClient cliente)
    {
        try
        {
            using var stream = cliente.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var mensaje = await reader.ReadLineAsync();
            if (mensaje is null)
            {
                return;
            }

            Console.WriteLine($"Cocina recibió: {mensaje}");

            var partes = mensaje.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var comando = partes.Length > 0 ? partes[0] : string.Empty;

            var respuesta = comando switch
            {
                "PREPARAR" => await PrepararAsync(partes),
                "PING" => "PONG",
                _ => $"ERROR comando desconocido: {mensaje}"
            };

            await WriteLineAsync(stream, respuesta);
            Console.WriteLine($"Cocina respondió: {respuesta}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cocina: error atendiendo cliente: {ex.Message}");
        }
    }

    private static async Task<string> PrepararAsync(string[] partes)
    {
        if (partes.Length < 2 || !int.TryParse(partes[1], out var pedidoId))
        {
            return "ERROR número de pedido inválido";
        }

        Console.WriteLine($"Cocina preparando pedido {pedidoId}...");
        await Task.Delay(2000);
        return $"LISTO {pedidoId}";
    }

    private static async Task WriteLineAsync(NetworkStream stream, string mensaje)
    {
        var bytes = Encoding.UTF8.GetBytes(mensaje + "\n");
        await stream.WriteAsync(bytes);
    }
}