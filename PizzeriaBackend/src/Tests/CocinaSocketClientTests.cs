using System.Net;
using System.Net.Sockets;
using System.Text;
using Business.Services;

namespace Tests;

public class CocinaSocketClientTests
{
    [Fact]
    public async Task PrepararPedidoAsync_CocinaRespondeListo_DevuelveRespuesta()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var puerto = ((IPEndPoint)listener.LocalEndpoint).Port;

        var servidorTask = AtenderUnaConexionAsync(listener, "LISTO 7");
        var cliente = new CocinaSocketClient("127.0.0.1", puerto);

        var respuesta = await cliente.PrepararPedidoAsync(7);
        var mensajeRecibido = await servidorTask;

        Assert.Equal("LISTO 7", respuesta);
        Assert.Equal("PREPARAR 7", mensajeRecibido);
    }

    [Fact]
    public async Task PrepararPedidoAsync_CocinaCaida_LanzaInvalidOperationException()
    {
        var cliente = new CocinaSocketClient("127.0.0.1", 5999);

        var excepcion = await Assert.ThrowsAsync<InvalidOperationException>(
            () => cliente.PrepararPedidoAsync(7));

        Assert.Contains("cocina no está disponible", excepcion.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<string> AtenderUnaConexionAsync(TcpListener listener, string respuesta)
    {
        using var socket = await listener.AcceptTcpClientAsync();
        using var stream = socket.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);

        var mensaje = await reader.ReadLineAsync() ?? string.Empty;
        var bytes = Encoding.UTF8.GetBytes(respuesta + "\n");
        await stream.WriteAsync(bytes);

        return mensaje;
    }
}